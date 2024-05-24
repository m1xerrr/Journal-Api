using Journal.DAL.Interfaces;
using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Microsoft.AspNetCore.Routing.Constraints;
using Newtonsoft.Json;

namespace Journal.DAL.Repositories
{
    public class MTDataRepository : IMTDataRepository
    {
        string baseUrl = "http://156.67.82.146:5000";
        public async Task<List<MTDealJsonModel>> GetDeals(MTAccountJsonModel account)
        {
           string endpoint = $"/get_deals?login={account.Login}&password={account.Password}&server={account.Server}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        List<MTDealJsonModel> deals = JsonConvert.DeserializeObject<List<MTDealJsonModel>>(content);
                        return deals;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return new List<MTDealJsonModel>();
        }

        public async Task<List<MTOrderJsonModel>> GetOrders(MTAccountJsonModel account)
        {
           string endpoint = $"/get_orders?login={account.Login}&password={account.Password}&server={account.Server}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response != null)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        List<MTOrderJsonModel> orders = JsonConvert.DeserializeObject<List<MTOrderJsonModel>>(content);
                        return orders;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<List<MTPositionJsonModel>> GetPositions(MTAccountJsonModel account)
        {
            string endpoint = $"/get_positions?login={account.Login}&password={account.Password}&server={account.Server}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response != null)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        List<MTPositionJsonModel> positions = JsonConvert.DeserializeObject<List<MTPositionJsonModel>>(content);
                        return positions;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<List<string>> GetSymbols(MTAccountJsonModel account)
        {
            string endpoint = $"/get_symbols?login={account.Login}&password={account.Password}&server={account.Server}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response != null)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        List<string> symbols = JsonConvert.DeserializeObject<List<string>>(content);
                        return symbols;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        public async Task<bool> PlaceOrder(int login, string password, string server, string symbol, float volume, byte type, float price, float stoploss, float takeprofit)
        {
            string endpoint = $"/place_order?login={login}&password={password}&server={server}&symbol={symbol}&volume={volume}&type={type}&price={price}&sl={stoploss}&tp={takeprofit}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);
                    string content = await response.Content.ReadAsStringAsync();
                    if (content.Contains("success"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return false;
        }

        public async Task<bool> DeleteOrder(int login, string password, string server, long ticket)
        {
            
            string endpointOrder = $"/close_order?login={login}&password={password}&server={server}&ticket={ticket}";
            string endpointPosition = $"/close_position?login={login}&password={password}&server={server}&ticket={ticket}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage responseOrder = await client.GetAsync(baseUrl + endpointOrder);
                    string contentOrder = await responseOrder.Content.ReadAsStringAsync();
                    if (contentOrder.Contains("success"))
                    {
                        return true;
                    }
                    else
                    {
                        HttpResponseMessage responsePosition = await client.GetAsync(baseUrl + endpointPosition);
                        string contentPosition = await responsePosition.Content.ReadAsStringAsync();
                        if (contentPosition.Contains("success"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return false;
        }

        public async Task<bool> Initialize(MTAccountJsonModel account)
        {
            string endpoint = $"/initialize?login={account.Login}&password={account.Password}&server={account.Server}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        if(content.Contains("Success"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return false;
        }
    }
}
