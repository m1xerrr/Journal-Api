using Journal.DAL.Interfaces;
using Journal.Domain.JsonModels;
using Newtonsoft.Json;

namespace Journal.DAL.Repositories
{
    public class MTDataRepository : IMTDataRepository
    {
        public async Task<List<MTDealJsonModel>> GetDeals(MTAccountJsonModel account)
        {
            string baseUrl = "http://10.125.40.15:5000";

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

        public async Task<bool> Initialize(MTAccountJsonModel account)
        {
            string baseUrl = "http://10.125.40.15:5000";

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
