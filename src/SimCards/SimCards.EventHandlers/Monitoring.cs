using Prometheus;

namespace SimCards.EventHandlers
{
    public class Monitoring : IMonitoring
    {
        private Counter simcardOrdersSentCounter;

        public Monitoring()
        {
            simcardOrdersSentCounter = Metrics.CreateCounter("simcard_orders_sent", "Number of SIM Card orders sent");
        }

        public void SendSimCardOrder()
        {
            simcardOrdersSentCounter.Inc(1);
        }
    }
}
