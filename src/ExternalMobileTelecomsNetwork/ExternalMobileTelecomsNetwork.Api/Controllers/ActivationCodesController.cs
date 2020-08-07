using ExternalMobileTelecomsNetwork.Api.Data;
using ExternalMobileTelecomsNetwork.Api.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Utils.DateTimes;

namespace ExternalMobileTelecomsNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivationCodesController : ControllerBase
    {
        private readonly IDataStore dataStore;
        private readonly IDateTimeCreator dateTimeCreator;

        public ActivationCodesController(IDataStore dataStore, 
            IDateTimeCreator dateTimeCreator)
        {
            this.dataStore = dataStore;
            this.dateTimeCreator = dateTimeCreator;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ActivationCodeToAdd activationCodeToAdd)
        {
            var isSuccess = false;
            using (dataStore.BeginTransaction())
            {
                var existing = dataStore.GetActivationCode(activationCodeToAdd.Reference);

                if (existing != null)
                {
                    isSuccess = dataStore.UpdateActivationCode(
                        new ActivationCode
                        {
                            Id = existing.Id,
                            Reference = existing.Reference,
                            Code = activationCodeToAdd.ActivationCode,
                            UpdatedAt = dateTimeCreator.Create()
                        });
                }
                else
                {
                    isSuccess = dataStore.InsertActivationCode(
                        new ActivationCode
                        {
                            Reference = activationCodeToAdd.Reference,
                            Code = activationCodeToAdd.ActivationCode
                        });
                }
            }

            if (!isSuccess)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            return new OkResult();
        }
    }
}