using System;
using System.Collections.Generic;

namespace LoadTestingSetupApp
{
    public class TestDataBuilder
    {
        private int globalCounter;

        public Data Build(TestDataSettings config)
        {
            var dataStore = new DataStore(config);

            var orderMobiles = new List<OrderMobileTestData>();
            var completeProvisions = new List<CompleteProvisionTestData>();
            var activateMobiles = new List<ActivateMobileTestData>();
            var completeActivates = new List<CompleteActivateTestData>();

            globalCounter = 1;

            BuildOrderMobiles(config, orderMobiles);
            BuildCompleteProvisions(config, dataStore, completeProvisions);
            BuildActivateMobiles(config, dataStore, activateMobiles);
            BuildCompleteActivates(config, dataStore, completeActivates);

            var data = new Data
            {
                OrderMobile = orderMobiles.ToArray(),
                CompleteProvision = completeProvisions.ToArray(),
                ActivateMobile = activateMobiles.ToArray(),
                CompleteActivate = completeActivates.ToArray()
            };
            return data;
        }

        private void BuildCompleteActivates(TestDataSettings config, DataStore dataStore, List<CompleteActivateTestData> completeActivates)
        {
            for (var i = 0; i < config.CompleteActivatesCount; i++)
            {
                var customerId = config.CustomerId;
                var mobileId = Guid.NewGuid().ToString();
                var activateOrderId = Guid.NewGuid().ToString();

                var phoneNumber = GetPhoneNumber(globalCounter);
                var newMobileDbId = dataStore.SetupDataForCompleteActivate(customerId, mobileId, activateOrderId, phoneNumber);
                completeActivates.Add(new CompleteActivateTestData
                {
                    CustomerId = customerId,
                    PhoneNumber = phoneNumber,
                    MobileId = mobileId,
                    ActivateOrderId = activateOrderId,
                    MobileDbId = newMobileDbId
                });
                globalCounter++;
            }
        }

        private void BuildActivateMobiles(TestDataSettings config, DataStore dataStore, List<ActivateMobileTestData> activateMobiles)
        {
            for (var i = 0; i < config.ActivateMobilesCount; i++)
            {
                var customerId = config.CustomerId;
                var mobileId = Guid.NewGuid().ToString();

                var phoneNumber = GetPhoneNumber(globalCounter);
                var activationCode = GetActivationCode(globalCounter);
                var newMobileDbId = dataStore.SetupDataForActivate(customerId, mobileId, phoneNumber, activationCode);
                activateMobiles.Add(new ActivateMobileTestData
                {
                    CustomerId = customerId,
                    PhoneNumber = phoneNumber,
                    MobileId = mobileId,
                    ActivationCode = activationCode,
                    MobileDbId = newMobileDbId
                });
                globalCounter++;
            }
        }

        private void BuildCompleteProvisions(TestDataSettings config, DataStore dataStore, List<CompleteProvisionTestData> completeProvisions)
        {
            for (var i = 0; i < config.CompleteProvisionsCount; i++)
            {
                var customerId = config.CustomerId;
                var mobileId = Guid.NewGuid().ToString();
                var provisionOrderId = Guid.NewGuid().ToString();
                var phoneNumber = GetPhoneNumber(globalCounter);
                var contactName = GetContactName(globalCounter);

                var newMobileDbId = dataStore.SetupDataForCompleteProvision(customerId, mobileId, provisionOrderId, phoneNumber,
                    contactName);
                completeProvisions.Add(new CompleteProvisionTestData
                {
                    CustomerId = customerId,
                    MobileId = mobileId,
                    ProvisionOrderId = provisionOrderId,
                    PhoneNumber = phoneNumber,
                    ContactName = contactName,
                    MobileDbId = newMobileDbId
                });
                globalCounter++;
            }
        }

        private void BuildOrderMobiles(TestDataSettings config, List<OrderMobileTestData> orderMobiles)
        {
            for (var i = 0; i < config.OrderMobilesCount; i++)
            {
                orderMobiles.Add(new OrderMobileTestData
                {
                    CustomerId = config.CustomerId,
                    PhoneNumber = GetPhoneNumber(globalCounter),
                    ContactName = GetContactName(globalCounter),
                    ContactPhoneNumber = GetContactPhoneNumber(globalCounter)
                });
                globalCounter++;
            }
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