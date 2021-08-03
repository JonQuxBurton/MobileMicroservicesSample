using System;
using Microsoft.Extensions.Options;
using SimCards.EventHandlers.Data;

namespace EndToEndApiLevelTests.DataAccess
{
    public class SimCardsData : Retry
    {
        private SimCardOrdersDataStore simCardOrdersDataStore;

        public SimCardsData(string connectionString)
        {
            var options = Options.Create<SimCards.EventHandlers.Config>(new SimCards.EventHandlers.Config()
            {
                ConnectionString = connectionString
            });
            simCardOrdersDataStore = new SimCardOrdersDataStore(options);
        }

        public SimCardOrder TryGetSimCardOrder(Guid mobileId, Guid mobileOrderId)
        {
            return TryGet(() => GetSimCardOrder(mobileId, mobileOrderId));
        }

        public SimCardOrder GetSimCardOrder(Guid mobileId, Guid mobileOrderId)
        {
            return simCardOrdersDataStore.GetExisting(mobileId, mobileOrderId);
        }
    }
}
