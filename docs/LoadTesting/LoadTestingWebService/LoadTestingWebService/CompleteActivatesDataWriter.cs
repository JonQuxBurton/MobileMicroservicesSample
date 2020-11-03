using System;
using System.Collections.Generic;
using LoadTestingWebService.Data;

namespace LoadTestingWebService
{
    public class CompleteActivatesDataWriter
    {
        public void Write(IDataStore dataStore, Dictionary<Guid, Dictionary<string, string>[]> data)
        {
            foreach (var dataForVirtualUsers in data.Values)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId =
                    dataStore.SetupDataForCompleteActivate(
                        dataForIteration["customerId"], 
                        dataForIteration["mobileId"],
                        dataForIteration["activateOrderId"], 
                        dataForIteration["phoneNumber"]);
                dataForIteration.Add("mobileDbId", newMobileDbId.ToString());
            }
        }
    }
}