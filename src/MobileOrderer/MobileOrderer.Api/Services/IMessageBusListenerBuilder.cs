using MinimalEventBus;

namespace MobileOrderer.Api.Services
{
    public interface IMessageBusListenerBuilder
    {
        IMessageBusListener Build();
    }
}
