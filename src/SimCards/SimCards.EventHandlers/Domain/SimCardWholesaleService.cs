using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimCards.EventHandlers.Domain
{
    public class SimCardWholesaleService : ISimCardWholesaleService
    {
        private readonly Config config;
        private readonly HttpClient httpClient;

        public SimCardWholesaleService(IOptions<Config> configOptions,
            HttpClient httpClient)
        {
            config = configOptions?.Value;
            this.httpClient = httpClient;
        }

        public async Task<bool> PostOrder(SimCardWholesalerOrder order)
        {
            var json = JsonConvert.SerializeObject(order);
            var response = await httpClient.PostAsync($"{config.SimCardWholesalerApiUrl}/api/orders", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
