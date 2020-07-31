using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using SimCards.EventHandlers.Domain;
using System;

namespace SimCards.EventHandlers.BackgroundServices
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
            try
            {
                logger.LogInformation("{ServiceName} starting...", ServiceName);
                messageBusListenerBuilder.Build().StartListening();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when {ServiceName} starting", ServiceName);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("{ServiceName} stopping...", ServiceName);
            return Task.CompletedTask;
        }

        private string ServiceName => GetType().Name;
    }
}
