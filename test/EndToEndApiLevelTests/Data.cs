using Polly;
using System;

namespace EndToEndApiLevelTests
{
    public class Data
    {
        public const int RetryAttempts = 18;
        private const int RetryIntervalSeconds = 10;

        protected T TryGet<T, U>(Func<U, T> func, U argument) where T : class
        {
            T result = null;
            var policy = Policy
              .Handle<Exception>()
              .WaitAndRetry(retryCount: RetryAttempts, retryNumber => TimeSpan.FromSeconds(RetryIntervalSeconds));

            policy.Execute(() =>
            {
                result = func(argument);

                if (result == null)
                    throw new Exception();

            });

            return result;
        }
    }
}
