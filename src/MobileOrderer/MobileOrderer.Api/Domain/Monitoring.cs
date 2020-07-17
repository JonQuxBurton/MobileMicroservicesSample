using Prometheus;

namespace MobileOrderer.Api.Domain
{
    public class Monitoring : IMonitoring
    {
        private readonly Counter provisionsCounter;
        private readonly Counter activatesCounter;
        private readonly Counter activatesCompletedCounter;
        private readonly Counter ceasesCounter;
        private readonly Counter ceasesCompletedCounter;

        public Monitoring()
        {
            provisionsCounter = Metrics.CreateCounter("mobile_provisions", "Number of Mobile Provisions");
            activatesCounter = Metrics.CreateCounter("mobile_activates", "Number of Mobile Activates");
            activatesCompletedCounter = Metrics.CreateCounter("mobile_activates_completed", "Number of Mobile Activates Completed");
            ceasesCounter = Metrics.CreateCounter("mobile_ceases", "Number of Mobile Ceases");
            ceasesCompletedCounter = Metrics.CreateCounter("mobile_ceases_completed", "Number of Mobile Ceases Completed");
        }

        public void Provision()
        {
            
            provisionsCounter.Inc(1);
        }
        
        public void Activate()
        {
            activatesCounter.Inc(1);
        }
        
        public void ActivateCompleted()
        {
            activatesCompletedCounter.Inc(1);
        }
        
        public void Cease()
        {
            ceasesCounter.Inc(1);
        }
        
        public void CeaseCompleted()
        {
            ceasesCompletedCounter.Inc(1);
        }
    }
}
