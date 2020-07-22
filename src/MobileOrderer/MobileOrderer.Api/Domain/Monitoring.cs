using Prometheus;

namespace MobileOrderer.Api.Domain
{
    public class Monitoring : IMonitoring
    {
        private readonly Counter provisions;
        private readonly Counter provisionsCompleted;
        private readonly Gauge provisionsInProgress;

        private readonly Counter activates;
        private readonly Counter activatesCompleted;
        private readonly Gauge activatesInProgress;

        private readonly Counter ceases;
        private readonly Counter ceasesCompletedCounter;
        private readonly Gauge ceasesInProgress;

        public Monitoring()
        {
            provisions = Metrics.CreateCounter("mobile_provisions", "Number of Mobile Provisions");
            provisionsCompleted = Metrics.CreateCounter("mobile_provisions_completed", "Number of Mobile Provisions completed");
            provisionsInProgress = Metrics.CreateGauge("mobile_provisions_inprogress", "Number of Mobile Provisions in progress");

            activates = Metrics.CreateCounter("mobile_activates", "Number of Mobile Activates");
            activatesCompleted = Metrics.CreateCounter("mobile_activates_completed", "Number of Mobile Activates completed");
            activatesInProgress = Metrics.CreateGauge("mobile_activates_inprogress", "Number of Mobile Activates in progress");

            ceases = Metrics.CreateCounter("mobile_ceases", "Number of Mobile Ceases");
            ceasesCompletedCounter = Metrics.CreateCounter("mobile_ceases_completed", "Number of Mobile Ceases completed");
            ceasesInProgress = Metrics.CreateGauge("mobile_ceases_inprogress", "Number of Mobile Ceases in progress");
        }

        public void Provision()
        {
            provisions.Inc(1);
            provisionsInProgress.Inc();
        }
        
        public void ProvisionCompleted()
        {
            provisionsCompleted.Inc();
            provisionsInProgress.Dec();
        }
        
        public void Activate()
        {
            activates.Inc(1);
            activatesInProgress.Inc();
        }
        
        public void ActivateCompleted()
        {
            activatesCompleted.Inc(1);
            activatesInProgress.Dec();
        }
        
        public void Cease()
        {
            ceases.Inc(1);
            ceasesInProgress.Inc();
        }
        
        public void CeaseCompleted()
        {
            ceasesCompletedCounter.Inc(1);
            ceasesInProgress.Dec();
        }
    }
}
