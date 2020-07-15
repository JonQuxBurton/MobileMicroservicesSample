using System;
using Microsoft.Extensions.Options;
using SimCards.EventHandlers.Data;

namespace EndToEndApiLevelTests.DataAcess
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

        public SimCardOrder TryGetSimCardOrder(Guid mobileOrderId)
        {
            return TryGet(() => GetSimCardOrder(mobileOrderId));
        }

        public SimCardOrder GetSimCardOrder(Guid mobileOrderId)
        {
            return simCardOrdersDataStore.GetExisting(mobileOrderId);
        }
    }
}
