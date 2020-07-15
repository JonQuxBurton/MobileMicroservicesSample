using System;
using SimCardWholesaler.Api.Data;
using Microsoft.Extensions.Options;

namespace EndToEndApiLevelTests.DataAcess
{
    public class ExternalSimCardOrdersData : Retry
    {
        private readonly OrdersDataStore ordersDataStore;

        public ExternalSimCardOrdersData(string connectionString)
        {
            var options = Options.Create<SimCardWholesaler.Api.Configuration.Config>(new SimCardWholesaler.Api.Configuration.Config()
            {
                ConnectionString = connectionString
            });

            ordersDataStore = new OrdersDataStore(options);
        }

        public Order TryGetExternalSimCardOrder(Guid reference)
        {
            return TryGet(() => GetNewExternalSimCardOrder(reference));
        }

        public Order GetNewExternalSimCardOrder(Guid reference)
        {
            var order = ordersDataStore.GetByReference(reference);

            if (order.Status.Trim() == "New")
                return order;
            
            return null;
        }
    }
}