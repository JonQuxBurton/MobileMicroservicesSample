using Polly;
using System;
using System.Threading;

namespace EndToEndApiLevelTests
{
    public class Data
    {
        protected T TryGet<T, U>(Func<U, T> func, U argument) where T : class
        {
            T target = null;
            Thread.Sleep(10 * 1000);

            var policy = Policy
              .Handle<Exception>()
              .WaitAndRetry(new[]
              {
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(40),
              });

            policy.Execute(() =>
            {
                target = func(argument);

                if (target == null)
                    throw new Exception();

            });

            return target;
        }
    }
}
