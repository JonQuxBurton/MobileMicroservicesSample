using ExternalMobileTelecomsNetwork.Api.Data;
using ExternalMobileTelecomsNetwork.Api.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Utils.DateTimes;

namespace ExternalMobileTelecomsNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivationCodesController : ControllerBase
    {
        private readonly ILogger<ActivationCodesController> logger;
        private readonly IDataStore dataStore;
        private readonly IDateTimeCreator dateTimeCreator;

        public ActivationCodesController(ILogger<ActivationCodesController> logger, 
            IDataStore dataStore, 
            IDateTimeCreator dateTimeCreator)
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.dateTimeCreator = dateTimeCreator;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ActivationCodeToAdd activationCodeToAdd)
        {
            var isSuccess = false;
            using (dataStore.BeginTransaction())
            {
                var existing = dataStore.GetActivationCode(activationCodeToAdd.PhoneNumber);

                if (existing != null)
                {
                    logger.LogDebug("Existing Activation Code for PhoneNumber {phoneNumber}", activationCodeToAdd.PhoneNumber);
                    var updatedAt = dateTimeCreator.Create();
                    logger.LogDebug("Updating Activation Code at {updatedAt}", updatedAt);

                    isSuccess = dataStore.UpdateActivationCode(
                        new ActivationCode
                        {
                            Id = existing.Id,
                            PhoneNumber = existing.PhoneNumber,
                            Code = activationCodeToAdd.ActivationCode,
                            UpdatedAt = updatedAt
                        });
                }
                else
                {
                    isSuccess = dataStore.InsertActivationCode(
                        new ActivationCode
                        {
                            PhoneNumber = activationCodeToAdd.PhoneNumber,
                            Code = activationCodeToAdd.ActivationCode
                        });
                }
            }

            if (!isSuccess)
            {
                logger.LogError("Failed to save ActivationCode for PhoneNumber {phoneNumber}", activationCodeToAdd.PhoneNumber);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkResult();
        }
    }
}