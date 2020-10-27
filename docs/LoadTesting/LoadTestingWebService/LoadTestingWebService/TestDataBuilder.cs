using System;

namespace LoadTestingWebService
{
    public class TestDataBuilder
    {
        private int globalCounter;

        public Data Build(TestDataSettings config)
        {
            var dataStore = new DataStore(config);

            var orderMobileTestData = new OrderMobileTestData[config.OrderMobilesSettings.VirtualUsers][];
            var completeProvisionsTestData =
                new CompleteProvisionTestData[config.CompleteProvisionsSettings.VirtualUsers][];
            var activateMobileTestData = new ActivateMobileTestData[config.ActivateMobilesSettings.VirtualUsers][];
            var completeActivatesTestData = new CompleteActivateTestData[config.CompleteActivatesSettings.VirtualUsers][];

            globalCounter = 1;

            BuildOrderMobiles(config, orderMobileTestData);
            BuildCompleteProvisions(config, dataStore, completeProvisionsTestData);
            BuildActivateMobiles(config, dataStore, activateMobileTestData);
            BuildCompleteActivates(config, dataStore, completeActivatesTestData);

            var data = new Data
            {
                OrderMobile = orderMobileTestData,
                CompleteProvision = completeProvisionsTestData,
                ActivateMobile = activateMobileTestData,
                CompleteActivate = completeActivatesTestData
            };
            return data;
        }

        private void BuildOrderMobiles(TestDataSettings config, OrderMobileTestData[][] orderMobileTestData)
        {
            for (var i = 0; i < config.OrderMobilesSettings.VirtualUsers; i++)
            {
                orderMobileTestData[i] = new OrderMobileTestData[config.OrderMobilesSettings.Iterations];

                for (var j = 0; j < config.OrderMobilesSettings.Iterations; j++)
                {
                    orderMobileTestData[i][j] = new OrderMobileTestData
                    {
                        CustomerId = config.CustomerId,
                        PhoneNumber = GetPhoneNumber(globalCounter),
                        ContactName = GetContactName(globalCounter),
                        ContactPhoneNumber = GetContactPhoneNumber(globalCounter)
                    };
                    globalCounter++;
                }
            }
        }

        private void BuildCompleteProvisions(TestDataSettings config, DataStore dataStore,
            CompleteProvisionTestData[][] completeProvisionsTestData)
        {
            for (var i = 0; i < config.OrderMobilesSettings.VirtualUsers; i++)
            {
                completeProvisionsTestData[i] =
                    new CompleteProvisionTestData[config.CompleteProvisionsSettings.Iterations];

                for (var j = 0; j < config.CompleteProvisionsSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();
                    var provisionOrderId = Guid.NewGuid().ToString();
                    var phoneNumber = GetPhoneNumber(globalCounter);
                    var contactName = GetContactName(globalCounter);

                    var newMobileDbId = dataStore.SetupDataForCompleteProvision(customerId, mobileId, provisionOrderId,
                        phoneNumber,
                        contactName);
                    completeProvisionsTestData[i][j] = new CompleteProvisionTestData
                    {
                        CustomerId = customerId,
                        MobileId = mobileId,
                        ProvisionOrderId = provisionOrderId,
                        PhoneNumber = phoneNumber,
                        ContactName = contactName,
                        MobileDbId = newMobileDbId
                    };
                    globalCounter++;
                }
            }
        }

        private void BuildActivateMobiles(TestDataSettings config, DataStore dataStore,
            ActivateMobileTestData[][] activateMobilesTestData)
        {
            for (var i = 0; i < config.ActivateMobilesSettings.VirtualUsers; i++)
            {
                activateMobilesTestData[i] =
                    new ActivateMobileTestData[config.ActivateMobilesSettings.Iterations];

                for (var j = 0; j < config.ActivateMobilesSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();

                    var phoneNumber = GetPhoneNumber(globalCounter);
                    var activationCode = GetActivationCode(globalCounter);
                    var newMobileDbId =
                        dataStore.SetupDataForActivate(customerId, mobileId, phoneNumber, activationCode);
                    activateMobilesTestData[i][j] = new ActivateMobileTestData
                    {
                        CustomerId = customerId,
                        PhoneNumber = phoneNumber,
                        MobileId = mobileId,
                        ActivationCode = activationCode,
                        MobileDbId = newMobileDbId
                    };
                    globalCounter++;
                }
            }
        }

        private void BuildCompleteActivates(TestDataSettings config, DataStore dataStore,
            CompleteActivateTestData[][] completeActivatesTestData)
        {
            for (var i = 0; i < config.CompleteActivatesSettings.VirtualUsers; i++)
            {
                completeActivatesTestData[i] = new CompleteActivateTestData[config.CompleteActivatesSettings.Iterations];
                for (var j = 0; j < config.CompleteActivatesSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();
                    var activateOrderId = Guid.NewGuid().ToString();

                    var phoneNumber = GetPhoneNumber(globalCounter);
                    var newMobileDbId =
                        dataStore.SetupDataForCompleteActivate(customerId, mobileId, activateOrderId, phoneNumber);
                    completeActivatesTestData[i][j] = new CompleteActivateTestData
                    {
                        CustomerId = customerId,
                        PhoneNumber = phoneNumber,
                        MobileId = mobileId,
                        ActivateOrderId = activateOrderId,
                        MobileDbId = newMobileDbId
                    };
                    globalCounter++;
                }
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