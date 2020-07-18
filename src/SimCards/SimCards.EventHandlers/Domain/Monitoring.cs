using Prometheus;

namespace SimCards.EventHandlers.Domain
{
    public class Monitoring : IMonitoring
    {
        private Counter simcardOrdersSent;
        private Counter simcardOrdersCompleted;

        public Monitoring()
        {
            simcardOrdersSent = Metrics.CreateCounter("simcard_orders_sent", "Number of SIM Card orders sent");
            simcardOrdersCompleted = Metrics.CreateCounter("simcard_orders_sent", "Number of SIM Card orders completed");
        }

        public void SimCardOrderCompleted()
        {
            simcardOrdersCompleted.Inc(1);
        }

        public void SimCardOrderSent()
        {
            simcardOrdersSent.Inc(1);
        }
    }
}
