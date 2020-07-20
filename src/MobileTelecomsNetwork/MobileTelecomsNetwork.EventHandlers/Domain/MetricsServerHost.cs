using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Prometheus;
using System.Threading;
using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class MetricsServerHost : IHostedService
    {
        private MetricServer metricServer;
        private Config config;

        public MetricsServerHost(IOptions<Config> configOptions)
        {
            config = configOptions.Value;
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
