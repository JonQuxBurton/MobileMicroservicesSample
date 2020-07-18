namespace SimCards.EventHandlers.Domain
{
    public interface IMonitoring
    {
        void SimCardOrderSent();
        void SimCardOrderCompleted();
    }
}