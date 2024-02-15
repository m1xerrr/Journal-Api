using Azure;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class MTDataRepository : IMTDataRepository
    {
        public async Task<List<DealJson>> GetDeals(MTAccountViewModel account)
        {
            string baseUrl = "http://10.125.41.146:5000";

            string endpoint = $"/get_deals?login={account.Login}&password={account.Password}&server={account.Server}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        List<DealJson> deals = JsonConvert.DeserializeObject<List<DealJson>>(content);
                        return deals;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return new List<DealJson>();
        }

        public async Task<bool> Initialize(MTAccountViewModel account)
        {
            string baseUrl = "http://10.125.41.146:5000";

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
