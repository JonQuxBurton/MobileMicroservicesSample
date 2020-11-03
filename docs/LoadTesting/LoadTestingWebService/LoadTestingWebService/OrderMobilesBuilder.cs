using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class OrderMobilesBuilder
    {
        public Dictionary<Guid, OrderMobileTestData[]> Build(TestDataSettings config)
        {
            var data = new Dictionary<Guid, OrderMobileTestData[]>();

            for (var i = 0; i < config.OrderMobilesSettings.VirtualUsers; i++)
            {
                var dataForIterations = new OrderMobileTestData[config.OrderMobilesSettings.Iterations];
                data.Add(Guid.NewGuid(), dataForIterations);

                for (var j = 0; j < config.OrderMobilesSettings.Iterations; j++)
                {
                    dataForIterations[j] = new OrderMobileTestData
                    {
                        CustomerId = config.CustomerId,
                        PhoneNumber = GetPhoneNumber(ScenariosService.GlobalCounter),
                        ContactName = GetContactName(ScenariosService.GlobalCounter),
                        ContactPhoneNumber = GetContactPhoneNumber(ScenariosService.GlobalCounter)
                    };
                    ScenariosService.GlobalCounter++;
                }
            }

            return data;
        }

        private string GetPhoneNumber(int counter)
        {
            return $"07001{counter.ToString().PadLeft(6, '0')}";
        }

        private string GetContactPhoneNumber(int counter)
        {
            return $"0114{counter.ToString().PadLeft(6, '0')}";
        }

        private string GetContactName(int counter)
        {
            return $"Neil Armstrong-{counter}";
        }

        private string GetActivationCode(int counter)
        {
            return $"AAA{counter.ToString().PadLeft(3, '0')}";
        }
    }
}