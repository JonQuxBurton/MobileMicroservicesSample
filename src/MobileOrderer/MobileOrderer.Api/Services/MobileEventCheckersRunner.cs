using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MobileOrderer.Api.Services
{
    public class MobileEventCheckersRunner : IMobileEventCheckersRunner
    {
        private readonly ILogger<MobileEventCheckersRunner> logger;
        private readonly IEnumerable<IMobileEventsChecker> checkers;

        public MobileEventCheckersRunner(ILogger<MobileEventCheckersRunner> logger,
            IEnumerable<IMobileEventsChecker> checkers)
        {
            this.logger = logger;
            this.checkers = checkers;
        }

        public async Task StartChecking(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Check();

                await Task.Delay(10 * 1000, stoppingToken);
            }
        }

        public void Check()
        {
            try
            {
                foreach (var checker in checkers)
                {
                    logger.LogInformation($"Checking {checker.GetType().Name}");
                    checker.Check();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception");
            }
        }
    }
}
