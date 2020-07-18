using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using MobileTelecomsNetwork.EventHandlers.Domain;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public class EventListenerHostedService : IHostedService
    {
        private readonly ILogger<EventListenerHostedService> logger;
        private readonly IMessageBusListenerBuilder messageBusListenerBuilder;

        public EventListenerHostedService(ILogger<EventListenerHostedService> logger, IMessageBusListenerBuilder messageBusListenerBuilder)
        {
            this.logger = logger;
            this.messageBusListenerBuilder = messageBusListenerBuilder;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("EventListenerHostedService Starting...");
            messageBusListenerBuilder.Build().StartListening();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("EventListenerHostedService Stopping...");
            return Task.CompletedTask;
        }
    }
}
