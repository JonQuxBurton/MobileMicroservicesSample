using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CompleteProvisionsDataWriter
    {
        public void Write(DataStore dataStore, Dictionary<Guid,CompleteProvisionTestData[]> data)
        {
            foreach (var dataForVirtualUsers in data.Values)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId = dataStore.SetupDataForCompleteProvision(dataForIteration.CustomerId,
                    dataForIteration.MobileId,
                    dataForIteration.ProvisionOrderId,
                    dataForIteration.PhoneNumber,
                    dataForIteration.ContactName);
                dataForIteration.MobileDbId = newMobileDbId;
            }
        }
    }
}