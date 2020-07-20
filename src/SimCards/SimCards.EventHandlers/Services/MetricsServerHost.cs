using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Prometheus;
using System.Threading;
using System.Threading.Tasks;

namespace SimCards.EventHandlers.Services
{
    public class MetricsServerHost : IHostedService
    {
        private MetricServer metricServer;
        private Config config;

        public MetricsServerHost(IOptions<Config> options)
        {
            config = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            metricServer = new MetricServer(port: config.MetricsServerHostPort);
            metricServer.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            metricServer.Stop();

            return Task.CompletedTask;
        }
    }
}
