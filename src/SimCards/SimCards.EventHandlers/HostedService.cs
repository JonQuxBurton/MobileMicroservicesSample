using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using SimCards.EventHandlers.Services;

namespace SimCards.EventHandlers
{
    public class HostedService : IHostedService
    {
        private readonly ILogger<HostedService> logger;
        private readonly IMessageBusListenerBuilder messageBusListenerBuilder;

        public HostedService(ILogger<HostedService> logger, IMessageBusListenerBuilder messageBusListenerBuilder)
        {
            this.logger = logger;
            this.messageBusListenerBuilder = messageBusListenerBuilder;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("HostedService Starting...");
            this.messageBusListenerBuilder.Build().StartListening();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("HostedService Stopping...");
            return Task.CompletedTask;
        }
    }
}
