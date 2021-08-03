using System;
using ExternalMobileTelecomsNetwork.Api.Data;
using Microsoft.Extensions.Options;

namespace EndToEndApiLevelTests.DataAccess
{
    public class ExternalMobileTelecomsNetworkData : Retry
    {
        private readonly DataStore dataStore;

        public ExternalMobileTelecomsNetworkData(string connectionString)
        {
            var options = Options.Create(new
                ExternalMobileTelecomsNetwork.Api.Configuration.Config()
            {
                ConnectionString = connectionString
            });

            dataStore = new DataStore(options);
        }

        public Order TryGetOrder(Guid mobileOrderId)
        {
            return TryGet(() => GetOrder(mobileOrderId));
        }

        public Order GetOrder(Guid reference)
        {
            return dataStore.GetByReference(reference);
        }
    }
}
