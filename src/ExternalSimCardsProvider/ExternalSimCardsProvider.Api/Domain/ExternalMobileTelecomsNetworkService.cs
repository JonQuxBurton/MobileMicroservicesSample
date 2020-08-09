using ExternalSimCardsProvider.Api.Configuration;
using ExternalSimCardsProvider.Api.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExternalSimCardsProvider.Api.Domain
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

        public async Task<bool> PostActivationCode(string phoneNumber, string activationCode)
        {
            var activationCodeToAdd = new ActivationCodeToAdd
            {
                PhoneNumber = phoneNumber,
                ActivationCode = activationCode
            };
            var json = JsonConvert.SerializeObject(activationCodeToAdd);
            var response = await httpClient.PostAsync($"{config.ExternalMobileTelecomsNetworkApiUrl}/api/activationcodes", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
