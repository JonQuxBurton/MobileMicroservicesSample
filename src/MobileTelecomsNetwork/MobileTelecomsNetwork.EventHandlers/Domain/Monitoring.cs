using Prometheus;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class Monitoring : IMonitoring
    {
        private Counter activateOrdersSent;
        private Counter activateOrdersCompleted;
        private Gauge activateOrdersInProgess;
        
        private Counter ceaseOrdersSent;
        private Counter ceaseOrdersCompleted;
        private Gauge ceaseOrdersInProgess;

        private Counter activateOrdersFailed;
        private Counter ceaseOrdersFailed;

        public Monitoring()
        {
            activateOrdersSent = Metrics.CreateCounter("mobiletelecomsnetwork_activate_orders_sent", "Number of MobileTelecomsNetwork Activate orders sent");
            activateOrdersCompleted = Metrics.CreateCounter("mobiletelecomsnetwork_activate_orders_completed", "Number of MobileTelecomsNetwork Activate orders completed");
            activateOrdersInProgess = Metrics.CreateGauge("mobiletelecomsnetwork_activate_orders_inprogress", "Number of MobileTelecomsNetwork Activate orders in progress");

            ceaseOrdersSent = Metrics.CreateCounter("mobiletelecomsnetwork_cease_orders_sent", "Number of MobileTelecomsNetwork Cease orders sent");
            ceaseOrdersCompleted = Metrics.CreateCounter("mobiletelecomsnetwork_cease_orders_completed", "Number of MobileTelecomsNetwork Cease orders completed");
            ceaseOrdersInProgess = Metrics.CreateGauge("mobiletelecomsnetwork_cease_orders_inprogress", "Number of MobileTelecomsNetwork Cease orders in progress");

            activateOrdersFailed = Metrics.CreateCounter("mobiletelecomsnetwork_activate_orders_failed", "Number of MobileTelecomsNetwork Activate orders failed");
            ceaseOrdersFailed = Metrics.CreateCounter("mobiletelecomsnetwork_cease_orders_failed", "Number of MobileTelecomsNetwork Cease orders failed");
        }

        public void ActivateOrderSent()
        {
            activateOrdersSent.Inc();
            activateOrdersInProgess.Inc();
        }

        public void ActivateOrderCompleted()
        {
            activateOrdersCompleted.Inc();
            activateOrdersInProgess.Dec();
        }

        public void ActivateOrderFailed()
        {
            activateOrdersFailed.Inc();
        }

        public void CeaseOrderSent()
        {
            ceaseOrdersSent.Inc();
            ceaseOrdersInProgess.Inc();
        }

        public void CeaseOrderCompleted()
        {
            ceaseOrdersCompleted.Inc();
            ceaseOrdersInProgess.Dec();
        }

        public void CeaseOrderFailed()
        {
            ceaseOrdersFailed.Inc();
        }
    }
}
