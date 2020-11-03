using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class ActivateMobilesBuilder
    {
        public Dictionary<Guid, ActivateMobileTestData[]> Build(TestDataSettings config)
        {
            var data = new Dictionary<Guid, ActivateMobileTestData[]>();

            for (var i = 0; i < config.ActivateMobilesSettings.VirtualUsers; i++)
            {
                var dataForIterations = new ActivateMobileTestData[config.ActivateMobilesSettings.Iterations];
                data.Add(Guid.NewGuid(), dataForIterations);

                for (var j = 0; j < config.ActivateMobilesSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();

                    var phoneNumber = GetPhoneNumber(ScenariosService.GlobalCounter);
                    var activationCode = GetActivationCode(ScenariosService.GlobalCounter);
                    dataForIterations[j] = new ActivateMobileTestData
                    {
                        CustomerId = customerId,
                        PhoneNumber = phoneNumber,
                        MobileId = mobileId,
                        ActivationCode = activationCode
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