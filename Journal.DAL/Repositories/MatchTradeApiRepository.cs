using Journal.DAL.Interfaces;
using Journal.Domain.JsonModels.MatchTrade;
using Journal.Domain.JsonModels.TradeLocker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Net.WebSockets;

namespace Journal.DAL.Repositories
{
    public class MatchTradeApiRepository : IMatchTradeApiRepository
    {
        string baseUrlDemo = "https://mtr-demo-prod.match-trader.com";
        string baseUrlLive = "";

        public async Task<List<MatchTradeDealJsonModel>> GetDeals(bool isLive, string email, string password, int brokerId, long accountNumber, string tradingApiToken, string coAuthToken)
        {
            string url = "wss://mtr-demo-prod.match-trader.com/app/snapshot/closed-positions";
            var client = new ClientWebSocket();
            client.Options.SetRequestHeader("Accept", "application/json");
            client.Options.SetRequestHeader("Content-Type", "application/json");
            client.Options.SetRequestHeader("Auth-trading-api", tradingApiToken);
            client.Options.SetRequestHeader("Cookie", $"co-auth={coAuthToken}");
            client.Options.SetRequestHeader("User-Agent", "PostmanRuntime/7.37.3");

            var cancellationToken = new CancellationTokenSource();
            cancellationToken.CancelAfter(TimeSpan.FromSeconds(30)); // 30 seconds timeout

            try
            {
                await client.ConnectAsync(new Uri(url), cancellationToken.Token);
                Console.WriteLine("WebSocket connection opened.");

                var requestBody = new
                {
                    symbols = new string[] { "BTCUSD", "ETHUSD" },
                    from = "2020-09-16T06:04:53.071Z",
                    to = "2020-09-23T06:04:53.071Z"
                };

                string requestJson = JsonConvert.SerializeObject(requestBody);
                var requestBytes = Encoding.UTF8.GetBytes(requestJson);
                var requestSegment = new ArraySegment<byte>(requestBytes);

                await client.SendAsync(requestSegment, WebSocketMessageType.Text, true, cancellationToken.Token);

                var buffer = new ArraySegment<byte>(new byte[2048]);
                WebSocketReceiveResult result = await client.ReceiveAsync(buffer, cancellationToken.Token);
                var responseString = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken.Token);
                    throw new WebSocketException("WebSocket connection closed unexpectedly.");
                }

                Console.WriteLine("Message received: " + responseString);
                var response = JsonConvert.DeserializeObject<MatchTradeDealsJsonModel>(responseString);

                foreach (var operation in response.Operations)
                {
                    Console.WriteLine($"Operation: {operation.Id}, Amount: {operation.Profit}");
                }

                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Completed", CancellationToken.None);

                return response.Operations;
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("WebSocket error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing WebSocket task: " + ex.Message);
                throw;
            }
            finally
            {
                if (client.State == WebSocketState.Open || client.State == WebSocketState.CloseReceived)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }

        public async Task<bool> OpenPosition(bool isLive, string email, string password, int brokerId, long accountNumber, double price, double stoploss, double takeprofit, double volume, byte type, string symbol)
        {

            var sessionData = await InitializeSession(isLive, email, password, brokerId, accountNumber);
            string side = "";
            string url = "";
            string json = "";

            if (type == 1 || type == 3)
            {
                side = "BUY";
            }
            else if (type == 2 || type == 4)
            {
                side = "SELL";
            }

            if (type < 3)
            {
                url = isLive ? baseUrlLive + $"/mtr-api/{sessionData.UUID}/position/open" : baseUrlDemo + $"/mtr-api/{sessionData.UUID}/position/open";
                json = $"{{\"instrument\":\"{symbol}\", \"orderSide\":\"{side}\", \"volume\":{volume}, \"slPrice\":{Math.Round(stoploss, 5)}, \"tpPrice\":{Math.Round(takeprofit, 5)}, \"isMobile\": \"true\"}}";

            }
            else
            {
                url = isLive ? baseUrlLive + $"/mtr-api/{sessionData.UUID}/pending-order/create" : baseUrlDemo + $"/mtr-api/{sessionData.UUID}/pending-order/create";
                json = $"{{\"instrument\":\"{symbol}\", \"orderSide\":\"{side}\", \"volume\":{volume}, \"price\":{Math.Round(price, 5)}, \"type\":\"LIMIT\", \"slPrice\":{Math.Round(stoploss, 5)}, \"tpPrice\":{Math.Round(takeprofit, 5)}, \"isMobile\": \"true\"}}";
            }

            var client = new HttpClient();

            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", $"co-auth={sessionData.CoAuthToken}");
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");
                client.DefaultRequestHeaders.Add("Auth-trading-api", sessionData.TradingApiToken);

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                OrderOpen sessionResponse = JsonConvert.DeserializeObject<OrderOpen>(responseBody);
                if (sessionResponse.Status == "OK") return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public async Task<bool> ClosePosition(bool isLive, string email, string password, int brokerId, long accountNumber, string positionId, string instrument, string side, string type, double volume)
        {

            var sessionData = await InitializeSession(isLive, email, password, brokerId, accountNumber);
            string url = "";
            string json = "";

            if (type != "LIMIT")
            {
                url = isLive ? baseUrlLive + $"/mtr-api/{sessionData.UUID}/positions/close" : baseUrlDemo + $"/mtr-api/{sessionData.UUID}/positions/close";
                ClosePositionData body = new ClosePositionData();
                body.PositionId = positionId;
                body.Instrument = instrument;
                body.OrderSide = side;
                body.Volume = volume;
                json = JsonConvert.SerializeObject(new[] { body });
            }
            else
            {
                url = isLive ? baseUrlLive + $"/mtr-api/{sessionData.UUID}/pending-order/cancel" : baseUrlDemo + $"/mtr-api/{sessionData.UUID}/pending-order/cancel";
                CloseOrderData body = new CloseOrderData();
                body.Instrument = instrument;
                body.Id = positionId;
                body.OrderSide = side;
                body.Type = type;
                body.IsMobile = "true";
                json = JsonConvert.SerializeObject(body);
            }

            var client = new HttpClient();

            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", $"co-auth={sessionData.CoAuthToken}");
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");
                client.DefaultRequestHeaders.Add("Auth-trading-api", sessionData.TradingApiToken);

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                OrderClose sessionResponse = JsonConvert.DeserializeObject<OrderClose>(responseBody);
                if (sessionResponse.Status == "OK") return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public async Task<double> GetDeposit(bool isLive, string email, string password, int brokerId, long accountNumber)
        {
            var sessionData = await InitializeSession(isLive, email, password, brokerId, accountNumber);

            string url = isLive ? baseUrlLive + $"/mtr-api/{sessionData.UUID}/last-finance" : baseUrlDemo + $"/mtr-api/{sessionData.UUID}/last-finance";
            string json = "{\"types\": [\"DEPOSIT\"], \"resultSize\": 100}";

            var client = new HttpClient();
            double deposit = 0;
            try
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", $"co-auth={sessionData.CoAuthToken}");
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");
                client.DefaultRequestHeaders.Add("Auth-trading-api", sessionData.TradingApiToken);
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    RootDeposit sessionResponse = JsonConvert.DeserializeObject<RootDeposit>(responseBody);

                    return deposit += sessionResponse.Operations.Select(x => x.Profit).Sum();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return deposit;
        }

        public async Task<MatchTradePositionsJsonModel> GetPositions(bool isLive, string email, string password, int brokerId, long accountNumber)
        {
            var sessionData = await InitializeSession(isLive, email, password, brokerId, accountNumber);

            string url = isLive ? baseUrlLive + $"/mtr-api/{sessionData.UUID}/open-positions" : baseUrlDemo + $"/mtr-api/{sessionData.UUID}/open-positions";

            var client = new HttpClient();
            try
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", $"co-auth={sessionData.CoAuthToken}");
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");
                client.DefaultRequestHeaders.Add("Auth-trading-api", sessionData.TradingApiToken);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    MatchTradePositionsJsonModel sessionResponse = JsonConvert.DeserializeObject<MatchTradePositionsJsonModel>(responseBody);

                    return sessionResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        public async Task<MatchTraderOrdersJsonModel> GetOrders(bool isLive, string email, string password, int brokerId, long accountNumber)
        {
            var sessionData = await InitializeSession(isLive, email, password, brokerId, accountNumber);

            string url = isLive ? baseUrlLive + $"/mtr-api/{sessionData.UUID}/active-orders" : baseUrlDemo + $"/mtr-api/{sessionData.UUID}/active-orders";

            var client = new HttpClient();
            try
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", $"co-auth={sessionData.CoAuthToken}");
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");
                client.DefaultRequestHeaders.Add("Auth-trading-api", sessionData.TradingApiToken);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    MatchTraderOrdersJsonModel sessionResponse = JsonConvert.DeserializeObject<MatchTraderOrdersJsonModel>(responseBody);

                    return sessionResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }
        public async Task<InitializeAccountJsonModel> InitializeSession(bool isLive, string email, string password, int brokerId, long accountNumber)
        { 

            string url = isLive ? baseUrlLive + "/mtr-backend/login" : baseUrlDemo + "/mtr-backend/login";
            string json = $"{{\"email\": \"{email}\", \"password\": \"{password}\", \"brokerId\": \"{brokerId}\"}}";

            var handler = new HttpClientHandler { UseCookies = true };
            var client = new HttpClient(handler);

            try
            {
               
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", "co-auth=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X3V1aWQiOiJkODRhZTk4Mi0zODhmLTQ1ZGItODM5NS0wZTRjYWM5YTIxZTEiLCJtYW5hZ2VyX3Njb3BlIjoiZDg0YWU5ODItMzg4Zi00NWRiLTgzOTUtMGU0Y2FjOWEyMWUxIiwidXNlcl9uYW1lIjoiaW50ZWdyYXRpb25AbXRyLmFwaTowIiwiZmwiOm51bGwsInNjb3BlIjpbXSwicGFydG5lcklkIjowLCJleHAiOjE3MTcyNDI0NDksImp0aSI6IjNlOTIwMzhiLTQyYzgtNGI3ZS05N2U4LTQ2NjI5MTRjY2ZiNCIsImVtYWlsIjoiaW50ZWdyYXRpb25AbXRyLmFwaSIsImF1dGhvcml0aWVzIjpbIkVNQUlMX1JFQUQiLCJQSE9ORV9SRUFEIiwiUk9MRV9VU0VSIiwiVVNFUiJdLCJjbGllbnRfaWQiOiJjbGllbnRJZCJ9.GDfKXCCZ0ldAVKEG8NO4jbZ6tWN5z3g7IZn9W19Xpss");
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    RootInitialize sessionResponse = JsonConvert.DeserializeObject<RootInitialize>(responseBody);

                    var cookies = handler.CookieContainer.GetCookies(new Uri(url));
                    string coAuthToken = cookies["co-auth"]?.Value;

                    InitializeAccountJsonModel model = new InitializeAccountJsonModel()
                    {
                        Id = accountNumber,
                        UUID = sessionResponse.TradingAccounts.FirstOrDefault(x => x.TradingAccountId == accountNumber.ToString()).Offer.System.Uuid,
                        TradingApiToken = sessionResponse.TradingAccounts.FirstOrDefault(x => x.TradingAccountId == accountNumber.ToString()).TradingApiToken,
                        CoAuthToken = coAuthToken
                    };

                    return model;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        private class Branch
        {
            [JsonProperty("uuid")]
            public string Uuid { get; set; }
        }

        private class TradingSystem
        {
            [JsonProperty("demo")]
            public bool Demo { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("uuid")]
            public string Uuid { get; set; }

            [JsonProperty("active")]
            public bool Active { get; set; }

            [JsonProperty("systemType")]
            public string SystemType { get; set; }

            [JsonProperty("tradingApiDomain")]
            public string TradingApiDomain { get; set; }
        }

        private class Offer
        {
            [JsonProperty("uuid")]
            public string Uuid { get; set; }

            [JsonProperty("partnerId")]
            public string PartnerId { get; set; }

            [JsonProperty("created")]
            public DateTime Created { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("currency")]
            public string Currency { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("initialDeposit")]
            public double InitialDeposit { get; set; }

            [JsonProperty("demo")]
            public bool Demo { get; set; }

            [JsonProperty("hidden")]
            public bool Hidden { get; set; }

            [JsonProperty("branch")]
            public Branch Branch { get; set; }

            [JsonProperty("system")]
            public TradingSystem System { get; set; }

            [JsonProperty("moneyManager")]
            public object MoneyManager { get; set; }

            [JsonProperty("leverage")]
            public int Leverage { get; set; }

            [JsonProperty("verificationRequired")]
            public bool VerificationRequired { get; set; }

            [JsonProperty("recordNumber")]
            public int RecordNumber { get; set; }

            [JsonProperty("mt5MamSystemType")]
            public object Mt5MamSystemType { get; set; }

            [JsonProperty("offerType")]
            public string OfferType { get; set; }
        }

        private class TradingAccountToken
        {
            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("expiration")]
            public DateTime Expiration { get; set; }
        }

        private class TradingAccount
        {
            [JsonProperty("tradingAccountId")]
            public string TradingAccountId { get; set; }

            [JsonProperty("offer")]
            public Offer Offer { get; set; }

            [JsonProperty("tradingApiToken")]
            public string TradingApiToken { get; set; }

            [JsonProperty("tradingAccountToken")]
            public TradingAccountToken TradingAccountToken { get; set; }

            [JsonProperty("branchUuid")]
            public string BranchUuid { get; set; }

            [JsonProperty("created")]
            public DateTime Created { get; set; }

            [JsonProperty("uuid")]
            public string Uuid { get; set; }
        }

        private class SelectedTradingAccount
        {
            [JsonProperty("tradingAccountId")]
            public string TradingAccountId { get; set; }

            [JsonProperty("offer")]
            public Offer Offer { get; set; }

            [JsonProperty("tradingApiToken")]
            public string TradingApiToken { get; set; }

            [JsonProperty("tradingAccountToken")]
            public TradingAccountToken TradingAccountToken { get; set; }

            [JsonProperty("branchUuid")]
            public string BranchUuid { get; set; }

            [JsonProperty("created")]
            public DateTime Created { get; set; }

            [JsonProperty("uuid")]
            public string Uuid { get; set; }

            [JsonProperty("group")]
            public string Group { get; set; }

            [JsonProperty("leverage")]
            public int Leverage { get; set; }

            [JsonProperty("isRetail")]
            public bool IsRetail { get; set; }

            [JsonProperty("isProView")]
            public bool IsProView { get; set; }
        }

        private class AccountInfo
        {
            [JsonProperty("uuid")]
            public string Uuid { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("surname")]
            public string Surname { get; set; }

            [JsonProperty("dateOfBirth")]
            public string DateOfBirth { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("postCode")]
            public string PostCode { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("branchUuid")]
            public string BranchUuid { get; set; }

            [JsonProperty("partnerId")]
            public string PartnerId { get; set; }

            [JsonProperty("bankAddress")]
            public string BankAddress { get; set; }

            [JsonProperty("bankSwiftCode")]
            public string BankSwiftCode { get; set; }

            [JsonProperty("bankAccount")]
            public string BankAccount { get; set; }

            [JsonProperty("bankName")]
            public string BankName { get; set; }

            [JsonProperty("state")]
            public string State { get; set; }

            [JsonProperty("role")]
            public string Role { get; set; }

            [JsonProperty("accountName")]
            public string AccountName { get; set; }

            [JsonProperty("faxNumber")]
            public string FaxNumber { get; set; }

            [JsonProperty("passportIdNumber")]
            public string PassportIdNumber { get; set; }

            [JsonProperty("passportIdCountry")]
            public string PassportIdCountry { get; set; }

            [JsonProperty("taxIdentificationNumber")]
            public string TaxIdentificationNumber { get; set; }

            [JsonProperty("citizenship")]
            public string Citizenship { get; set; }

            [JsonProperty("maritalStatus")]
            public string MaritalStatus { get; set; }

            [JsonProperty("guestAccount")]
            public bool GuestAccount { get; set; }
        }

        private class TfaProperties
        {
            [JsonProperty("enabled")]
            public bool Enabled { get; set; }

            [JsonProperty("required")]
            public bool Required { get; set; }
        }

        private class TfaConfig
        {
            [JsonProperty("isEnabledForSignIn")]
            public bool IsEnabledForSignIn { get; set; }
        }

        private class RootInitialize
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("tradingAccounts")]
            public List<TradingAccount> TradingAccounts { get; set; }

            [JsonProperty("selectedTradingAccount")]
            public SelectedTradingAccount SelectedTradingAccount { get; set; }

            [JsonProperty("accountInfo")]
            public AccountInfo AccountInfo { get; set; }

            [JsonProperty("accessMode")]
            public string AccessMode { get; set; }

            [JsonProperty("tfaProperties")]
            public TfaProperties TfaProperties { get; set; }

            [JsonProperty("tfaConfig")]
            public TfaConfig TfaConfig { get; set; }
        }

        private class Operation
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("time")]
            public DateTime Time { get; set; }

            [JsonProperty("profit")]
            public double Profit { get; set; }

            [JsonProperty("comment")]
            public string Comment { get; set; }

            [JsonProperty("uid")]
            public string Uid { get; set; }
        }

        private class RootDeposit
        {
            [JsonProperty("operations")]
            public List<Operation> Operations { get; set; }
        }

        private class OrderOpen
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("nativeCode")]
            public string NativeCode { get; set; }

            [JsonProperty("errorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty("orderId")]
            public string OrderId { get; set; }
        }

        private class OrderClose
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("nativeCode")]
            public string NativeCode { get; set; }

            [JsonProperty("errorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty("orderId")]
            public string OrderId { get; set; }
        }

        private class ClosePositionData
        {
            [JsonProperty("positionId")]
            public string PositionId { get; set; }
            [JsonProperty("instrument")]
            public string Instrument { get; set; }
            [JsonProperty("orderSide")]
            public string OrderSide { get; set; }
            [JsonProperty("volume")]
            public double Volume { get; set; }
        }

        private class CloseOrderData
        {

            [JsonProperty("instrument")]
            public string Instrument { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
            
            [JsonProperty("orderSide")]
            public string OrderSide { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("isMobile")]
            public string IsMobile { get; set; }

        }
    }
}
