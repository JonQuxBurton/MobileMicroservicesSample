using System;
using ExternalSimCardsProvider.Api.Data;
using Microsoft.Extensions.Options;

namespace EndToEndApiLevelTests.DataAccess
{
    public class ExternalSimCardOrdersData : Retry
    {
        private readonly OrdersDataStore ordersDataStore;

        public ExternalSimCardOrdersData(string connectionString)
        {
            var options = Options.Create<ExternalSimCardsProvider.Api.Configuration.Config>(new ExternalSimCardsProvider.Api.Configuration.Config()
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