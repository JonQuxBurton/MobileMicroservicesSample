using System;
using Microsoft.Extensions.Options;

namespace LoadTestingWebService
{
    public class DataGenerator : IDataGenerator
    {
        private readonly TestDataSettings testDataSettings;
        private readonly ConcurrentCounter counter = new ConcurrentCounter();

        public DataGenerator(IOptions<TestDataSettings> testDataSettingsOptions)
        {
            testDataSettings = testDataSettingsOptions.Value;
        }

        public string GetExistingCustomerId()
        {
            return testDataSettings.CustomerId;
        }

        public string GetNextPhoneNumber()
        {
            return $"07001{counter.Next().ToString().PadLeft(6, '0')}";
        }

        public string GetNextContactPhoneNumber()
        {
            return $"0114{counter.Next().ToString().PadLeft(6, '0')}";
        }

        public string GetNextContactName()
        {
            return $"Neil Armstrong-{counter.Next()}";
        }

        public string GetNextActivationCode()
        {
            return $"AAA{counter.Next().ToString().PadLeft(3, '0')}";
        }

        public Guid GetNextGuid()
        {
            return Guid.NewGuid();
        }
    }
}
