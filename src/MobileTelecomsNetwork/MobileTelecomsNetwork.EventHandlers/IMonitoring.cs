namespace MobileTelecomsNetwork.EventHandlers
{
    public interface IMonitoring
    {
        void ActivateOrderSent();
        void CeaseOrderSent();
        void ActivateOrderCompleted();
        void CeaseOrderCompleted();
    }
}