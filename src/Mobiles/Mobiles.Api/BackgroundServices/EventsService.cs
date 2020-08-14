using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mobiles.Api.Services
{
    public class EventsService : IHostedService
    {
        private readonly ILogger<EventsService> logger;
        private readonly IMessageBusListenerBuilder messageBusListenerBuilder;

        public EventsService(ILogger<EventsService> logger, 
            IMessageBusListenerBuilder messageBusListenerBuilder)
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
