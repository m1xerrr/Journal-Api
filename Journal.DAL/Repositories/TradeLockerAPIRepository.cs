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
                        Console.WriteLine("Response: " + responseBody);

                        SessionResponse sessionResponse = JsonConvert.DeserializeObject<SessionResponse>(responseBody);

                        sessionToken = sessionResponse.AccessToken;
                        DateTime timeout = sessionResponse.ExpireDate;
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

            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public DateTime ExpireDate { get; set; }
        }
    }
}
