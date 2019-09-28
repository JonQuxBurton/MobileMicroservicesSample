using MinimalEventBus;

namespace SimCards.EventHandlers.Services
{
    public interface IMessageBusListenerBuilder
    {
        IMessageBusListener Build();
    }
}