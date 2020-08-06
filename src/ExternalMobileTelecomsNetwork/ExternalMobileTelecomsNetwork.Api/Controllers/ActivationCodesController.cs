using ExternalMobileTelecomsNetwork.Api.Data;
using ExternalMobileTelecomsNetwork.Api.Resources;
using Microsoft.AspNetCore.Mvc;

namespace ExternalMobileTelecomsNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivationCodesController : ControllerBase
    {
        private readonly IDataStore dataStore;

        public ActivationCodesController(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ActivationCodeToAdd activationCodeToAdd)
        {
            using (dataStore.BeginTransaction())
            {
                dataStore.AddActivationCode(activationCodeToAdd);
            }

            return new OkResult();
        }
    }
}