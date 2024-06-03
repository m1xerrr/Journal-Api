using Journal.DAL.Interfaces;
using Newtonsoft.Json;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Journal.Domain.JsonModels.TradeLocker;
using System.Drawing;

namespace Journal.DAL.Repositories
{
    public class TradeLockerAPIRepository : ITradeLockerAPIRepository
    {
        string baseUrlDemo = "https://demo.tradelocker.com/backend-api";
        string baseUrlLive = "https://live.tradelocker.com/backend-api";

        public async Task<TradeLockerApiAccountJsonModel> GetAccount(string email, string password, string server, bool isLive, long accountId)
        {
                string baseUrl;
                if(isLive) baseUrl = baseUrlLive;
                else baseUrl = baseUrlDemo;

                string endpoint = $"/auth/jwt/all-accounts";

                string sessionToken = await Initialize(email, password, server, isLive);

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                        HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonContent = await response.Content.ReadAsStringAsync();

                            var container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        return container.Accounts.FirstOrDefault(x => x.Id == accountId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                return null;
        }

        public async Task<double> GetDeposit(string email, string password, string server, bool isLive, long accountId)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            double deposit = 0;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        deposit += container.Accounts.FirstOrDefault(x => x.Id == accountId).AccountBalance;

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/trade/reports/closed-positions-history?startTime=0&endTime=9999999999999");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var info = JsonConvert.DeserializeObject<TradeLockerAPIDealsJsonModel>(jsonContent);
                        deposit -= info.TotalNetProfit;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return deposit;
        }

        public async Task<TradeLockerSymbolJsonModel> GetSymbols(string email, string password, string server, bool isLive, long accountId)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            double deposit = 0;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        deposit += container.Accounts.FirstOrDefault(x => x.Id == accountId).AccountBalance;

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    HttpResponseMessage response = await client.GetAsync(baseUrl + $"/trade/accounts/{accountId}/instruments");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var info = JsonConvert.DeserializeObject<TradeLockerSymbolJsonModel>(jsonContent);
                        return info;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<TradeLockerOrdersJsonModel> GetOrders(string email, string password, string server, bool isLive, long accountId)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            double deposit = 0;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        deposit += container.Accounts.FirstOrDefault(x => x.Id == accountId).AccountBalance;

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    HttpResponseMessage response = await client.GetAsync(baseUrl + $"/trade/accounts/{accountId}/orders");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var info = JsonConvert.DeserializeObject<TradeLockerOrdersJsonModel>(jsonContent);
                        return info;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<double> GetPrice(string symbol, string email = "alihuim1xar1@gmail.com", string password = "gCvDDs7$", string server = "OSP-DEMO", bool isLive = false, long accountId = 518920)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            double deposit = 0;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        deposit += container.Accounts.FirstOrDefault(x => x.Id == accountId).AccountBalance;

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    
                    var symbolInfo = (await GetSymbols(email, password, server, isLive, accountId)).Data.Instruments.FirstOrDefault(x => x.Name == symbol);
                    if (symbolInfo == null) return 0;
                    

                    int routeId = symbolInfo.Routes.FirstOrDefault(x => x.Type == "INFO").Id;
                    int tradableInstrumentId = symbolInfo.TradableInstrumentId;

                    string url = $"{baseUrl}/trade/dailyBar?routeId={routeId}&barType=ASK&tradableInstrumentId={tradableInstrumentId}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var dailyBar = JsonConvert.DeserializeObject<DailyBarResponse>(jsonContent);
                        if (dailyBar.Status == "ok") return dailyBar.Data.CPrice;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return 0;
        }

        public async Task<TradeLockerPositionsJsonModel> GetPositions(string email, string password, string server, bool isLive, long accountId)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            double deposit = 0;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        deposit += container.Accounts.FirstOrDefault(x => x.Id == accountId).AccountBalance;

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    HttpResponseMessage response = await client.GetAsync(baseUrl + $"/trade/accounts/{accountId}/positions");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var info = JsonConvert.DeserializeObject<TradeLockerPositionsJsonModel>(jsonContent);
                        return info;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<bool> PlaceOrder(string email, string password, string server, bool isLive, long accountId, double price, double stoploss, double takeprofit, double volume, byte type, string symbol)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            double deposit = 0;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        deposit += container.Accounts.FirstOrDefault(x => x.Id == accountId).AccountBalance;

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    string side = "";
                    string limitType = "";
                    string validity = "";

                    switch (type)
                    {
                        case 1:
                            side = "buy";
                            limitType = "market";
                            validity = "IOC";
                            break;
                        case 2:
                            side = "sell";
                            limitType = "market";
                            validity = "IOC";
                            break;
                        case 3:
                            side = "buy";
                            limitType = "limit";
                            validity = "GTC";
                            break;
                        case 4:
                            side = "sell";
                            limitType = "limit";
                            validity = "GTC";
                            break;
                        default:
                            return false;
                    }
                    var symbolInfo = (await GetSymbols(email, password, server, isLive, accountId)).Data.Instruments.FirstOrDefault(x => x.Name == symbol);
                    if(symbolInfo == null) return false;
                    string json = $"{{\"price\": {price}, \"qty\": {volume}, \"routeId\": {symbolInfo.Routes.FirstOrDefault(x => x.Type == "TRADE").Id}, \"side\": \"{side}\", \"stopLoss\": {stoploss}, \"stopLossType\": \"absolute\", \"stopPrice\": {0}, \"takeProfit\": {takeprofit}, \"takeProfitType\": \"absolute\", \"trStopOffset\": {0}, \"tradableInstrumentId\": {symbolInfo.TradableInstrumentId}, \"type\": \"{limitType}\", \"validity\": \"{validity}\"}}"; 

                    HttpResponseMessage response = await client.PostAsync(baseUrl + $"/trade/accounts/{accountId}/orders", new StringContent(json, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        if (jsonContent.ToLower().Contains("ok")) return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return false;
        }

        public async Task<bool> DeleteOrder(string email, string password, string server, bool isLive, long accountId, long positionId)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            double deposit = 0;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        deposit += container.Accounts.FirstOrDefault(x => x.Id == accountId).AccountBalance;

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    HttpResponseMessage response = await client.GetAsync(baseUrl + $"/trade/accounts/{accountId}/positions");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var info = JsonConvert.DeserializeObject<TradeLockerPositionsJsonModel>(jsonContent);
                        if(info.Data.Positions.FirstOrDefault(x => long.Parse(x[0].ToString()) == positionId) != null)
                        {
                            string json = $"{{\"qty\": {double.Parse(info.Data.Positions.FirstOrDefault(x => long.Parse(x[0].ToString()) == positionId)[4].ToString())}}}";
                            var request = new HttpRequestMessage(HttpMethod.Delete, baseUrl + $"/trade/positions/{positionId}")
                            {
                                Content = new StringContent(json, Encoding.UTF8, "application/json")
                            };
                            HttpResponseMessage responsePosition = await client.SendAsync(request);
                            if (responsePosition.IsSuccessStatusCode)
                            {
                                string contentPosition = await responsePosition.Content.ReadAsStringAsync();
                                if (contentPosition.ToLower().Contains("ok")) return true;
                            }
                        }
                    }

                    HttpResponseMessage responseOrder = await client.DeleteAsync(baseUrl + $"/trade/orders/{positionId}");

                    if (responseOrder.IsSuccessStatusCode)
                    {
                        string jsonContent = await responseOrder.Content.ReadAsStringAsync();
                        if (jsonContent.ToLower().Contains("ok")) return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return false;
        }

        public async Task<TradeLockerAPIDealsJsonModel> GetDeals(string email, string password, string server, bool isLive, long accountId)
        {
            string baseUrl;
            if (isLive) baseUrl = baseUrlLive;
            else baseUrl = baseUrlDemo;

            string sessionToken = await Initialize(email, password, server, isLive);

            int accNum = 0;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/auth/jwt/all-accounts");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        TradeLockerApiAccountsContainerJsonModel container = JsonConvert.DeserializeObject<TradeLockerApiAccountsContainerJsonModel>(jsonContent);

                        accNum = container.Accounts.FirstOrDefault(x => x.Id == accountId).AccNum;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionToken}");
                    client.DefaultRequestHeaders.Add("accNum", accNum.ToString());

                    HttpResponseMessage response = await client.GetAsync(baseUrl + "/trade/reports/closed-positions-history?startTime=0&endTime=9999999999999");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var deals = JsonConvert.DeserializeObject<TradeLockerAPIDealsJsonModel>(jsonContent);
                        return deals;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }
        public async Task<string> Initialize(string email, string password, string server, bool isLive)
        {
            string sessionToken = null;
            string json = $"{{\"email\": \"{email}\", \"password\": \"{password}\", \"server\": \"{server}\"}}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    string url;
                    if(isLive) url = baseUrlLive;
                    else url = baseUrlDemo;
                    HttpResponseMessage response = await client.PostAsync(url + "/auth/jwt/token", new StringContent(json, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        SessionResponse sessionResponse = JsonConvert.DeserializeObject<SessionResponse>(responseBody);

                        sessionToken = sessionResponse.AccessToken;
                        DateTime timeout = sessionResponse.ExpireDate;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return sessionToken;

        }

        private class SessionResponse
        {

            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public DateTime ExpireDate { get; set; }
        }

        private class DailyBarResponse
        {
            [JsonProperty("d")]
            public DailyBarData Data { get; set; }
            [JsonProperty("s")]
            public string Status { get; set; }
        }
        private class DailyBarData
        {
            [JsonProperty("o")]
            public double OPrice { get; set; }

            [JsonProperty("h")]
            public double HPrice { get; set; }

            [JsonProperty("l")]
            public double LPrice { get; set; }

            [JsonProperty("c")]
            public double CPrice { get; set; }

            [JsonProperty("v")]
            public double? VPrice { get; set; }
        }
    }
}
