using Prometheus;

namespace SimCards.EventHandlers.Domain
{
    public class Monitoring : IMonitoring
    {
        private Counter simcardOrdersSent;
        private Counter simcardOrdersCompleted;
        private Gauge simcardOrdersInProgress;

        private Counter simcardOrdersFailed;

        public Monitoring()
        {
            simcardOrdersSent = Metrics.CreateCounter("simcard_orders_sent", "Number of SIM Card orders sent");
            simcardOrdersCompleted = Metrics.CreateCounter("simcard_orders_completed", "Number of SIM Card orders completed");
            simcardOrdersInProgress = Metrics.CreateGauge("simcard_orders_inprogress", "Number of SIM Card orders in proress");

            simcardOrdersFailed = Metrics.CreateCounter("simcard_orders_failed", "Number of SIM Card orders failed");
        }

        public void SimCardOrderSent()
        {
            simcardOrdersSent.Inc();
            simcardOrdersInProgress.Inc();
        }

        public void SimCardOrderCompleted()
        {
            simcardOrdersCompleted.Inc(1);
            simcardOrdersInProgress.Dec();
        }

        public void SimCardOrderFailed()
        {
            simcardOrdersFailed.Inc();
        }
    }
}
