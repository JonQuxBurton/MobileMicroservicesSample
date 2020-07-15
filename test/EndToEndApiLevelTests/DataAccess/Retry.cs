using Polly;
using System;

namespace EndToEndApiLevelTests.DataAcess
{
    public class Retry
    {
        public const int RetryAttempts = 18;
        private const int RetryIntervalSeconds = 10;

        protected T TryGet<T>(Func<T> func) where T : class
        {
            T result = null;
            var policy = Policy
              .Handle<Exception>()
              .WaitAndRetry(retryCount: RetryAttempts, retryNumber => TimeSpan.FromSeconds(RetryIntervalSeconds));

            policy.Execute(() =>
            {
                result = func();

                if (result == null)
                    throw new Exception();

            });

            return result;
        }
    }
}
