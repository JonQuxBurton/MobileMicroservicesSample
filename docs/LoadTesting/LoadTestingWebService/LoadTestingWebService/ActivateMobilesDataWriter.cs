using System;
using System.Collections.Generic;
using LoadTestingWebService.Data;

namespace LoadTestingWebService
{
    public class ActivateMobilesDataWriter
    {
        public void Write(IDataStore dataStore, Dictionary<Guid, Dictionary<string, string>[]> data)
        {
            foreach (var dataForVirtualUsers in data.Values)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId =
                    dataStore.SetupDataForActivate(
                        dataForIteration["customerId"],
                        dataForIteration["mobileId"],
                        dataForIteration["phoneNumber"],
                        dataForIteration["activationCode"]);
                dataForIteration.Add("mobileDbId", newMobileDbId.ToString());
            }
        }
    }
}