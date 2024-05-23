using CloudflareSolverRe;
using Journal.DAL.Interfaces;
using Journal.Domain.JsonModels.DXTrade;
using Journal.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class DXTradeDataRepository : IDXTradeDataRepository
    {
        public async Task<DXTradeAccountAPIUserJsonModel> GetAccounts(string username, string password, string domain)
        {
            string baseUrl = $"https://{domain}/dxsca-web";

            string endpoint = $"/users/{username}";

            string sessionToken = await Initialize(username, password, baseUrl);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DXAPI", sessionToken);
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        DXTradeAccountAPIUserDetailsContainer container = JsonConvert.DeserializeObject<DXTradeAccountAPIUserDetailsContainer>(jsonContent);

                        return container.UserDetails.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<DXTradeAPIDealJsonModel> GetDeals(string username, string password, string domain, string account)
        {
            string baseUrl = $"https://{domain}/dxsca-web";

            string endpoint = $"/accounts/default:{account}/orders/history";

            string sessionToken = await Initialize(username, password, baseUrl);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DXAPI", sessionToken);
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        DXTradeAPIDealJsonModel container = JsonConvert.DeserializeObject<DXTradeAPIDealJsonModel>(jsonContent);

                        return container;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<double> GetDeposit(string username, string password, string domain, string account)
        {
            string baseUrl = $"https://{domain}/dxsca-web";

            string endpoint = $"/accounts/default:{account}/transfers";

            string sessionToken = await Initialize(username, password, baseUrl);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DXAPI", sessionToken);
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        CashTransfetRootObject container = JsonConvert.DeserializeObject<CashTransfetRootObject>(jsonContent);

                        double deposit = container.CashTransfers.Where(x => x.Comment == "Deposit").Sum(x => x.CashTransactions[0].Value);

                        return deposit;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return 0.0;
        }

        public async Task<string> Initialize(string username, string password, string url)
        {
            string sessionToken = null;
            string json = $"{{\"username\": \"{username}\", \"domain\": \"default\", \"password\": \"{password}\"}}";

            var target = new Uri(url+"/login");

            var cf = new CloudflareSolver
            {
                MaxTries = 3,
                ClearanceDelay = 3000
            };

            var handler = new HttpClientHandler();


            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var result = cf.Solve(client, handler, target).Result;

                    HttpResponseMessage response = await client.PostAsync(url + "/login", new StringContent(json, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response: " + responseBody);

                        SessionResponse sessionResponse = JsonConvert.DeserializeObject<SessionResponse>(responseBody);

                        sessionToken = sessionResponse.SessionToken;
                        TimeSpan timeout = sessionResponse.Timeout;

                        Console.WriteLine("Session Token: " + sessionToken);
                        Console.WriteLine("Timeout: " + timeout);
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
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
            public string SessionToken { get; set; }
            public TimeSpan Timeout { get; set; }
        }

        private class CashTransaction
        {
            public string Account { get; set; }
            public string TransactionCode { get; set; }
            public string OrderCode { get; set; }
            public string TradeCode { get; set; }
            public int Version { get; set; }
            public string ClientOrderId { get; set; }
            public string Type { get; set; }
            public double Value { get; set; }
            public string Currency { get; set; }
            public DateTime TransactionTime { get; set; }
        }

        private class CashTransfer
        {
            public string Account { get; set; }
            public int Version { get; set; }
            public string TransferCode { get; set; }
            public string Comment { get; set; }
            public DateTime TransactionTime { get; set; }
            public List<CashTransaction> CashTransactions { get; set; }
        }

        private class CashTransfetRootObject
        {
            public List<CashTransfer> CashTransfers { get; set; }
        }
    }
}
