using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace MobileOrderer.Api.Services
{
    public class EventsService : IHostedService
    {
        private readonly ILogger<EventsService> logger;
        private readonly IMessageBusListenerBuilder messageBusListenerBuilder;

        public EventsService(ILogger<EventsService> logger, IMessageBusListenerBuilder messageBusListenerBuilder)
        {
            this.logger = logger;
            this.messageBusListenerBuilder = messageBusListenerBuilder;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("EventsService Starting...");
            this.messageBusListenerBuilder.Build().StartListening();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("EventsService Stopping...");
            return Task.CompletedTask;
        }
    }
}
