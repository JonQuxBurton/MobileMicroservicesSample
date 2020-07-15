namespace EndToEndApiLevelTests.DataAcess
{
    public class Data
    {
        public Data(Config config)
        {
            MobilesData = new MobilesData(config.ConnectionString);
            MobileTelecomsNetworkData = new MobileTelecomsNetworkData(config.ConnectionString);
            ExternalMobileTelecomsNetworkData = new ExternalMobileTelecomsNetworkData(config.ConnectionString);
            SimCardsData = new SimCardsData(config.ConnectionString);
            ExternalSimCardOrdersData = new ExternalSimCardOrdersData(config.ConnectionString);

        }

        public MobilesData MobilesData { get; }
        public MobileTelecomsNetworkData MobileTelecomsNetworkData { get; }
        public ExternalMobileTelecomsNetworkData ExternalMobileTelecomsNetworkData { get; }
        public SimCardsData SimCardsData { get; }
        public ExternalSimCardOrdersData ExternalSimCardOrdersData { get; }
    }
}
