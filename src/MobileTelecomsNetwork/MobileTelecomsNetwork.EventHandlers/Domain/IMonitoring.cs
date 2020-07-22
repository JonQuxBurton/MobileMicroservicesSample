namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public interface IMonitoring
    {
        void ActivateOrderSent();
        void CeaseOrderSent();
        void ActivateOrderCompleted();
        void CeaseOrderCompleted();
        void ActivateOrderFailed();
        void CeaseOrderFailed();
    }
}