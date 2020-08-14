using MinimalEventBus;

namespace Mobiles.Api.Services
{
    public interface IMessageBusListenerBuilder
    {
        IMessageBusListener Build();
    }
}
