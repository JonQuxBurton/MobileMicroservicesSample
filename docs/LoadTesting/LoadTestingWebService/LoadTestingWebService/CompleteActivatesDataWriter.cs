using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CompleteActivatesDataWriter
    {
        public void Write(DataStore dataStore, Dictionary<Guid, CompleteActivateTestData[]> data)
        {
            foreach (var dataForVirtualUsers in data.Values)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId =
                    dataStore.SetupDataForCompleteActivate(dataForIteration.CustomerId, dataForIteration.MobileId,
                        dataForIteration.ActivateOrderId, dataForIteration.PhoneNumber);
                dataForIteration.MobileDbId = newMobileDbId;
            }
        }
    }
}