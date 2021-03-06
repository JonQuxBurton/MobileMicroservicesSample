﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.BackgroundServices
{
    public class MetricsServerHost : IHostedService
    {
        private MetricServer metricServer;
        private readonly Config config;
        private readonly ILogger<MetricsServerHost> logger;

        public MetricsServerHost(IOptions<Config> configOptions, ILogger<MetricsServerHost> logger)
        {
            config = configOptions.Value;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("{ServiceName} starting...", ServiceName);
                metricServer = new MetricServer(port: config.MetricsServerHostPort);
                metricServer.Start();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when {ServiceName} starting", ServiceName);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("{ServiceName} stopping...", ServiceName);
                metricServer.Stop();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when {ServiceName} stopping", ServiceName);
            }

            return Task.CompletedTask;
        }

        private string ServiceName => GetType().Name;
    }
}
