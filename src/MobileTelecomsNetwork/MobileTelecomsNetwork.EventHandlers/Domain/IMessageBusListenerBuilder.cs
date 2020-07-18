using MinimalEventBus;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public interface IMessageBusListenerBuilder
    {
        IMessageBusListener Build();
    }
}