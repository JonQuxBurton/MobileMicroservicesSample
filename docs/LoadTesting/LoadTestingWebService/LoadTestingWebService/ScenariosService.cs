using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LoadTestingWebService.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoadTestingWebService
{
    public class ScenariosService
    {
        private static readonly ConcurrentDictionary<string, Scenario>
            Scenarios = new ConcurrentDictionary<string, Scenario>();

        public static int GlobalCounter = 1;
        private readonly TestDataSettings testDataSettings;

        public ScenariosService(TestDataSettings testDataSettings)
        {
            this.testDataSettings = testDataSettings;
            var dataStore = new DataStore(testDataSettings);

            var virtualUserGlobalIds = new List<Guid>();
            for (var i = 0; i < testDataSettings.OrderMobilesSettings.VirtualUsers; i++)
                virtualUserGlobalIds.Add(new Guid());

            var scenarioName = "orderMobile";
            //var scenario = new Scenario(scenarioName, virtualUserGlobalIds);
            var scenario = new Scenario(scenarioName);
            Scenarios.TryAdd(scenarioName, scenario);

            Console.WriteLine("Generating data...");
            var orderMobilesBuilder = new OrderMobilesBuilder();
            var dataForOrderMobiles = orderMobilesBuilder.Build(testDataSettings);

            var completeProvisionsBuilder = new CompleteProvisionsBuilder();
            var dataForCompleteProvisions = completeProvisionsBuilder.Build(testDataSettings);

            var activateMobilesBuilder = new ActivateMobilesBuilder();
            var dataForActivateMobiles = activateMobilesBuilder.Build(testDataSettings);

            var completeActivatesBuilder = new CompleteActivatesBuilder();
            var dataForCompleteActivates = completeActivatesBuilder.Build(testDataSettings);

            Console.WriteLine("Writing data to Database...");
            var completeProvisionsDataWriter = new CompleteProvisionsDataWriter();
            completeProvisionsDataWriter.Write(dataStore, dataForCompleteProvisions);

            var activateMobilesDataWriter = new ActivateMobilesDataWriter();
            activateMobilesDataWriter.Write(dataStore, dataForActivateMobiles);

            var completeActivatesDataWriter = new CompleteActivatesDataWriter();
            completeActivatesDataWriter.Write(dataStore, dataForCompleteActivates);

            Console.WriteLine($"Writing data to load testing data file ({testDataSettings.FileNameData})...");
            var allData = new Data
            {
                OrderMobile = dataForOrderMobiles,
                CompleteProvision = dataForCompleteProvisions,
                ActivateMobile = dataForActivateMobiles,
                CompleteActivate = dataForCompleteActivates
            };
            var json = ConvertToJson(allData);
            File.WriteAllText(Path.Combine(testDataSettings.Path, testDataSettings.FileNameData), json);

            Console.WriteLine($"Writing data to Scenario Script file ({testDataSettings.FileNameScenarios})...");
            
            File.WriteAllText(
                Path.Combine(testDataSettings.Path, testDataSettings.FileNameScenarios),
                GenerateScenarioText(allData));

            Console.WriteLine("Done!");
        }

        private static string ConvertToJson(Data data)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            return json;
        }

        private static string GenerateScenarioText(Data data)
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append("==================================================");
            builder.AppendLine();

            if (data.OrderMobile.Any())
            {
                foreach (var orderMobileTestData in data.OrderMobile[0])
                {
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: OrderMobile
--------------------------------------------------
DATA
CustomerId:         {orderMobileTestData.CustomerId}
PhoneNumber:        {orderMobileTestData.PhoneNumber}
ContactName:        {orderMobileTestData.ContactName}
ContactPhoneNumber: {orderMobileTestData.ContactPhoneNumber}
--------------------------------------------------
AT END
Mobile created with
    State:      'ProcessingProvision'
SimCards Order created with
    Status:     'Sent'
ExternalSimCardsProvider Order created with
    Status:     'New'
--------------------------------------------------
";
                    builder.Append(scenariosTextTemplate);
                }

                builder.AppendLine();
                builder.Append("==================================================");
                builder.AppendLine();
            }

            if (data.CompleteProvision.Any())
            {
                foreach (var orderMobileTestData in data.CompleteProvision[0])
                {
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: CompleteProvision
--------------------------------------------------
DATA
CustomerId:         {orderMobileTestData.CustomerId}
PhoneNumber:        {orderMobileTestData.PhoneNumber}
MobileId:           {orderMobileTestData.MobileId}
--------------------------------------------------
AT START
SimCards Order has
    Status:     'Sent'
ExternalSimCardsProvider has
    Status:     'New'
--------------------------------------------------
AT END
Mobile created with
    State:      'WaitingForActivation'
SimCards Order update to
    Status:     'Completed'
ExternalSimCardsProvider Order update to
    Status:     'Completed'
--------------------------------------------------
";
                    builder.Append(scenariosTextTemplate);
                }

                builder.AppendLine();
                builder.Append("==================================================");
                builder.AppendLine();
            }

            if (data.ActivateMobile.Any())
            {
                foreach (var activateMobileTestData in data.ActivateMobile[0])
                {
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: ActivateMobile
--------------------------------------------------
DATA
CustomerId:     {activateMobileTestData.CustomerId}
PhoneNumber:    {activateMobileTestData.PhoneNumber}
MobileId:       {activateMobileTestData.MobileId}
ActivationCode: {activateMobileTestData.ActivationCode}
--------------------------------------------------
AT START
Mobile has
    State:      'WaitingForActivate'
--------------------------------------------------
AT END
Mobile updated to
    State:      'ProcessingActivate'
MobileTelecomsNetwork Order created with
    MobileId    {activateMobileTestData.MobileDbId}
    Status:     'Sent'
ExternalTelecomsNetwork Order created with
    Status:     'New'
--------------------------------------------------
";
                    builder.Append(scenariosTextTemplate);
                }

                builder.AppendLine();
                builder.Append("==================================================");
                builder.AppendLine();
            }

            if (data.CompleteActivate.Any())
                foreach (var completeActivateTestData in data.CompleteActivate[0])
                {
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: CompleteActivate
--------------------------------------------------
DATA
CustomerId:     {completeActivateTestData.CustomerId}
PhoneNumber:    {completeActivateTestData.PhoneNumber}
MobileId:       {completeActivateTestData.MobileId}
--------------------------------------------------
AT START
Mobile has
    State:      'ProcessingActivate'
MobileTelecomsNetwork Order has
    Status:     'Sent'
ExternalTelecomsNetwork has
    Status:     'New'
--------------------------------------------------
AT END
Mobile updated to
    State:      'Live'
MobileTelecomsNetwork Order updated to
    MobileId    {completeActivateTestData.MobileDbId}
    Status:     'Completed'
ExternalTelecomsNetwork Order created with
    Status:     'Completed'
--------------------------------------------------
";
                    builder.Append(scenariosTextTemplate);
                }

            return builder.ToString();
        }
    }

    public class OrderMobilesBuilder
    {
        public OrderMobileTestData[][] Build(TestDataSettings config)
        {
            var orderMobileTestData = new OrderMobileTestData[config.OrderMobilesSettings.VirtualUsers][];

            for (var i = 0; i < config.OrderMobilesSettings.VirtualUsers; i++)
            {
                orderMobileTestData[i] = new OrderMobileTestData[config.OrderMobilesSettings.Iterations];

                for (var j = 0; j < config.OrderMobilesSettings.Iterations; j++)
                {
                    orderMobileTestData[i][j] = new OrderMobileTestData
                    {
                        CustomerId = config.CustomerId,
                        PhoneNumber = GetPhoneNumber(ScenariosService.GlobalCounter),
                        ContactName = GetContactName(ScenariosService.GlobalCounter),
                        ContactPhoneNumber = GetContactPhoneNumber(ScenariosService.GlobalCounter)
                    };
                    ScenariosService.GlobalCounter++;
                }
            }

            return orderMobileTestData;
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

    public class CompleteProvisionsBuilder
    {
        public CompleteProvisionTestData[][] Build(TestDataSettings config)
        {
            var data = new CompleteProvisionTestData[config.OrderMobilesSettings.VirtualUsers][];

            for (var i = 0; i < config.OrderMobilesSettings.VirtualUsers; i++)
            {
                data[i] =
                    new CompleteProvisionTestData[config.CompleteProvisionsSettings.Iterations];

                for (var j = 0; j < config.CompleteProvisionsSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();
                    var provisionOrderId = Guid.NewGuid().ToString();
                    var phoneNumber = GetPhoneNumber(ScenariosService.GlobalCounter);
                    var contactName = GetContactName(ScenariosService.GlobalCounter);

                    data[i][j] = new CompleteProvisionTestData
                    {
                        CustomerId = customerId,
                        MobileId = mobileId,
                        ProvisionOrderId = provisionOrderId,
                        PhoneNumber = phoneNumber,
                        ContactName = contactName
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

    public class ActivateMobilesBuilder
    {
        public ActivateMobileTestData[][] Build(TestDataSettings config)
        {
            var data = new ActivateMobileTestData[config.ActivateMobilesSettings.VirtualUsers][];

            for (var i = 0; i < config.OrderMobilesSettings.VirtualUsers; i++)
            {
                data[i] =
                    new ActivateMobileTestData[config.ActivateMobilesSettings.Iterations];

                for (var j = 0; j < config.ActivateMobilesSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();

                    var phoneNumber = GetPhoneNumber(ScenariosService.GlobalCounter);
                    var activationCode = GetActivationCode(ScenariosService.GlobalCounter);
                    data[i][j] = new ActivateMobileTestData
                    {
                        CustomerId = customerId,
                        PhoneNumber = phoneNumber,
                        MobileId = mobileId,
                        ActivationCode = activationCode
                        //MobileDbId = newMobileDbId
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
    
    public class CompleteActivatesBuilder
    {
        public CompleteActivateTestData[][] Build(TestDataSettings config)
        {
            var data = new CompleteActivateTestData[config.CompleteActivatesSettings.VirtualUsers][];

            for (var i = 0; i < config.OrderMobilesSettings.VirtualUsers; i++)
            {
                data[i] =
                    new CompleteActivateTestData[config.ActivateMobilesSettings.Iterations];

                for (var j = 0; j < config.ActivateMobilesSettings.Iterations; j++)
                {
                    var customerId = config.CustomerId;
                    var mobileId = Guid.NewGuid().ToString();
                    var activateOrderId = Guid.NewGuid().ToString();

                    var phoneNumber = GetPhoneNumber(ScenariosService.GlobalCounter);
                    data[i][j] = new CompleteActivateTestData
                    {
                        CustomerId = customerId,
                        PhoneNumber = phoneNumber,
                        MobileId = mobileId,
                        ActivateOrderId = activateOrderId,
                        //MobileDbId = newMobileDbId
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

    public class CompleteProvisionsDataWriter
    {
        public void Write(DataStore dataStore, CompleteProvisionTestData[][] data)
        {
            foreach (var dataForVirtualUsers in data)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId = dataStore.SetupDataForCompleteProvision(dataForIteration.CustomerId,
                    dataForIteration.MobileId,
                    dataForIteration.ProvisionOrderId,
                    dataForIteration.PhoneNumber,
                    dataForIteration.ContactName);
                dataForIteration.MobileDbId = newMobileDbId;
            }
        }
    }

    public class ActivateMobilesDataWriter
    {
        public void Write(DataStore dataStore, ActivateMobileTestData[][] data)
        {
            foreach (var dataForVirtualUsers in data)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId =
                    dataStore.SetupDataForActivate(dataForIteration.CustomerId, dataForIteration.MobileId, dataForIteration.PhoneNumber, dataForIteration.ActivationCode);
                dataForIteration.MobileDbId = newMobileDbId;
            }
        }
    }

    public class CompleteActivatesDataWriter
    {
        public void Write(DataStore dataStore, CompleteActivateTestData[][] data)
        {
            foreach (var dataForVirtualUsers in data)
            foreach (var dataForIteration in dataForVirtualUsers)
            {
                var newMobileDbId =
                    dataStore.SetupDataForCompleteActivate(dataForIteration.CustomerId, dataForIteration.MobileId, dataForIteration.ActivateOrderId, dataForIteration.PhoneNumber);
                dataForIteration.MobileDbId = newMobileDbId;
            }
        }
    }
}