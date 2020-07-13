using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public class ExternalMobileTelecomsNetworkService : IExternalMobileTelecomsNetworkService
    {
        private readonly Config config;
        private readonly HttpClient httpClient;

        public ExternalMobileTelecomsNetworkService(IOptions<Config> configOptions, HttpClient httpClient)
        {
            config = configOptions?.Value;
            this.httpClient = httpClient;
        }

        public async Task<bool> PostOrder(ExternalMobileTelecomsNetworkOrder order)
        {
            var json = JsonConvert.SerializeObject(order);
            var response = await httpClient.PostAsync($"{config.ExternalMobileTelecomsNetworkApiUrl}/api/orders", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> PostCancel(ExternalMobileTelecomsNetworkOrder order)
        {
            var response = await httpClient.DeleteAsync($"{config.ExternalMobileTelecomsNetworkApiUrl}/api/orders/{order.Reference}");

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
