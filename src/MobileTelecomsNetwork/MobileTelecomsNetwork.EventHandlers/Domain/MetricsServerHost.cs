using Microsoft.Extensions.Hosting;
using Prometheus;
using System.Threading;
using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class MetricsServerHost : IHostedService
    {
        private MetricServer metricServer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            metricServer = new MetricServer(port: 80);
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
