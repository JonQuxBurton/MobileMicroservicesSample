using MinimalEventBus;

namespace SimCards.EventHandlers.Domain
{
    public interface IMessageBusListenerBuilder
    {
        IMessageBusListener Build();
    }
}