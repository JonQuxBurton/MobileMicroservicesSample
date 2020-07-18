using Prometheus;

namespace MobileTelecomsNetwork.EventHandlers
{
    public class Monitoring : IMonitoring
    {
        private Counter activateOrdersSent;
        private Counter activateOrdersCompleted;
        private Counter ceaseOrdersSent;
        private Counter ceaseOrdersCompleted;

        public Monitoring()
        {
            activateOrdersSent = Metrics.CreateCounter("mobiletelecomsnetwork_activate_orders_sent", "Number of MobileTelecomsNetwork Activate orders sent");
            activateOrdersCompleted = Metrics.CreateCounter("mobiletelecomsnetwork_activate_orders_completed", "Number of MobileTelecomsNetwork Activate orders completed");
            ceaseOrdersSent = Metrics.CreateCounter("mobiletelecomsnetwork_cease_orders_sent", "Number of MobileTelecomsNetwork Cease orders sent");
            ceaseOrdersCompleted = Metrics.CreateCounter("mobiletelecomsnetwork_cease_orders_completed", "Number of MobileTelecomsNetwork Cease orders completed");
        }

        public void ActivateOrderCompleted()
        {
            activateOrdersCompleted.Inc(1);
        }

        public void ActivateOrderSent()
        {
            activateOrdersSent.Inc(1);
        }

        public void CeaseOrderCompleted()
        {
            ceaseOrdersCompleted.Inc(1);
        }

        public void CeaseOrderSent()
        {
            ceaseOrdersSent.Inc(1);
        }
    }
}
