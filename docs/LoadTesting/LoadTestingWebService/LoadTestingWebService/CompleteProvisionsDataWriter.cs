using System;
using System.Collections.Generic;
using LoadTestingWebService.Data;

namespace LoadTestingWebService
{
    public class CompleteProvisionsDataWriter
    {
        public void Write(IDataStore dataStore, Dictionary<Guid, Dictionary<string, string>[]> data)
        {
            foreach (var dataForVirtualUsers in data.Values)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId = dataStore.SetupDataForCompleteProvision(dataForIteration["customerId"],
                    dataForIteration["mobileId"],
                    dataForIteration["provisionOrderId"],
                    dataForIteration["phoneNumber"],
                    dataForIteration["contactName"]);
                dataForIteration.Add("mobileDbId", newMobileDbId.ToString());
            }
        }
    }
}