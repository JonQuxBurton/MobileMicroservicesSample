﻿using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class CompleteActivatesBuilder
    {
        public Dictionary<Guid, Dictionary<string, string>[]> Build(TestDataSettings config)
        {
            var data = new Dictionary<Guid, Dictionary<string, string>[]>();

            for (var i = 0; i < config.CompleteActivatesSettings.VirtualUsers; i++)
            {
                var dataForIterations = new Dictionary<string, string>[config.CompleteActivatesSettings.Iterations];
                data.Add(Guid.NewGuid(), dataForIterations);

                for (var j = 0; j < config.ActivateMobilesSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();
                    var activateOrderId = Guid.NewGuid().ToString();

                    var phoneNumber = GetPhoneNumber(ScenariosService.GlobalCounter);
                    dataForIterations[j] = new Dictionary<string, string>
                    {
                        {"customerId", customerId },
                        {"phoneNumber", phoneNumber },
                        {"mobileId", mobileId },
                        {"activateOrderId", activateOrderId }
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