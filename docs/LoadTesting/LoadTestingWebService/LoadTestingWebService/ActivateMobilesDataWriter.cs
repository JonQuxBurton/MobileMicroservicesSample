using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class ActivateMobilesDataWriter
    {
        public void Write(DataStore dataStore, Dictionary<Guid, ActivateMobileTestData[]> data)
        {
            foreach (var dataForVirtualUsers in data.Values)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId =
                    dataStore.SetupDataForActivate(dataForIteration.CustomerId, dataForIteration.MobileId,
                        dataForIteration.PhoneNumber, dataForIteration.ActivationCode);
                dataForIteration.MobileDbId = newMobileDbId;
            }
        }
    }
}