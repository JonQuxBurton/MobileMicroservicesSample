using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace MobileOrderer.Api.Services
{
    public class EventPublisherService : BackgroundService
    {
        private readonly ILogger<EventPublisherService> logger;
        private readonly IMobileEventCheckersRunner mobileEventCheckersRunner;

        public EventPublisherService(ILogger<EventPublisherService> logger,
            IMobileEventCheckersRunner mobileEventCheckersRunner)
        {
            this.logger = logger;
            this.mobileEventCheckersRunner = mobileEventCheckersRunner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("EventPublisherService is starting...");

            await mobileEventCheckersRunner.StartChecking(stoppingToken);
        }
    }
}
