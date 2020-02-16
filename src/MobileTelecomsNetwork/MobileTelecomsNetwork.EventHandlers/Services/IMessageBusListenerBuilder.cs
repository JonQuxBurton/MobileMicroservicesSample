using MinimalEventBus;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public interface IMessageBusListenerBuilder
    {
        IMessageBusListener Build();
    }
}