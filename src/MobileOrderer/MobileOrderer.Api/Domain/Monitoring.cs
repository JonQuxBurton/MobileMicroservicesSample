using Prometheus;

namespace MobileOrderer.Api.Domain
{
    public class Monitoring : IMonitoring
    {
        private readonly Counter provisions;
        private readonly Counter provisionsCompleted;
        private readonly Counter activates;
        private readonly Counter activatesCompleted;
        private readonly Counter ceases;
        private readonly Counter ceasesCompletedCounter;

        public Monitoring()
        {
            provisions = Metrics.CreateCounter("mobile_provisions", "Number of Mobile Provisions");
            provisionsCompleted = Metrics.CreateCounter("mobile_provisions_completed", "Number of Mobile Provisions completed");
            activates = Metrics.CreateCounter("mobile_activates", "Number of Mobile Activates");
            activatesCompleted = Metrics.CreateCounter("mobile_activates_completed", "Number of Mobile Activates Completed");
            ceases = Metrics.CreateCounter("mobile_ceases", "Number of Mobile Ceases");
            ceasesCompletedCounter = Metrics.CreateCounter("mobile_ceases_completed", "Number of Mobile Ceases Completed");
        }

        public void Provision()
        {
            
            provisions.Inc(1);
        }
        
        public void ProvisionCompleted()
        {
            
            provisionsCompleted.Inc(1);
        }
        
        public void Activate()
        {
            activates.Inc(1);
        }
        
        public void ActivateCompleted()
        {
            activatesCompleted.Inc(1);
        }
        
        public void Cease()
        {
            ceases.Inc(1);
        }
        
        public void CeaseCompleted()
        {
            ceasesCompletedCounter.Inc(1);
        }
    }
}
