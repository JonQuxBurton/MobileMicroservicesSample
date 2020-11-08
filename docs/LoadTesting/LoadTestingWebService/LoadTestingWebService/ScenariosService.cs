using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LoadTestingWebService.Resources;
using Microsoft.Extensions.Options;

namespace LoadTestingWebService
{
    public class ScenariosService : IScenariosService
    {
        private readonly IDataFileWriter dataFileWriter;
        private readonly IDataGenerator dataGenerator;
        private readonly IDataStoreWriter dataStoreWriter;

        private readonly object locker = new object();
        private readonly IScenariosDataBuilder scenariosDataBuilder;
        private readonly IScenarioScriptFileWriter scenarioTextGenerator;
        private readonly TestDataSettings testDataSettings;

        public ConcurrentDictionary<string, VirtualUserRegistrations> Registrations =
            new ConcurrentDictionary<string, VirtualUserRegistrations>();

        public ConcurrentDictionary<string, List<Guid>> VirtualUserGlobalIds =
            new ConcurrentDictionary<string, List<Guid>>();

        public ScenariosService(IOptions<TestDataSettings> testDataSettingsOptions,
            IScenariosDataBuilder scenariosDataBuilder,
            IScenarioScriptFileWriter scenarioTextGenerator,
            IDataGenerator dataGenerator,
            IDataFileWriter dataFileWriter,
            IDataStoreWriter dataStoreWriter)
        {
            testDataSettings = testDataSettingsOptions.Value;
            this.scenariosDataBuilder = scenariosDataBuilder;
            this.scenarioTextGenerator = scenarioTextGenerator;
            this.dataGenerator = dataGenerator;
            this.dataFileWriter = dataFileWriter;
            this.dataStoreWriter = dataStoreWriter;
        }

        public void GenerateData()
        {
            var scenarios = new List<Scenario>
            {
                new CreateCustomerScenario(testDataSettings.CreateCustomersSettings.VirtualUsers,
                    testDataSettings.CreateCustomersSettings.Iterations, false, false),
                new OrderMobileScenario(testDataSettings.OrderMobilesSettings.VirtualUsers,
                    testDataSettings.OrderMobilesSettings.Iterations,
                    true,
                    false,
                    dataGenerator),
                new CompleteProvisionScenario(testDataSettings.CompleteProvisionsSettings.VirtualUsers,
                    testDataSettings.CompleteProvisionsSettings.Iterations,
                    true,
                    true,
                    dataGenerator),
                new CompleteActivateScenario(testDataSettings.CompleteActivatesSettings.VirtualUsers,
                    testDataSettings.CompleteActivatesSettings.Iterations,
                    true,
                    true,
                    dataGenerator),
                new ActivateMobileScenario(testDataSettings.ActivateMobilesSettings.VirtualUsers,
                    testDataSettings.ActivateMobilesSettings.Iterations,
                    true,
                    true,
                    dataGenerator)
            };

            Console.WriteLine("Generating data...");

            var dataForScenarios = scenariosDataBuilder.Build(scenarios);
            StoreVirtualUserGlobalIds(dataForScenarios);

            Console.WriteLine("Writing data to Database...");

            var dataForScenariosWithDatabaseIds = dataStoreWriter.Write(scenarios, dataForScenarios);

            Console.WriteLine($"Writing data to load testing data file ({testDataSettings.FileNameData})...");

            var dataInFormatForFile = GetDataInFormatForFile(scenarios, dataForScenariosWithDatabaseIds);
            dataFileWriter.WriteDataFile(dataInFormatForFile);

            Console.WriteLine($"Writing data to Scenario Script file ({testDataSettings.FileNameScenarios})...");

            scenarioTextGenerator.Write(dataInFormatForFile);

            Console.WriteLine("Done!");
        }

        public UserResource GetUserId(string scenarioKey, int virtualUserId)
        {
            UserResource user;

            lock (locker)
            {
                if (!Registrations.ContainsKey(scenarioKey))
                    Registrations.TryAdd(scenarioKey, new VirtualUserRegistrations(VirtualUserGlobalIds[scenarioKey]));

                var registration = Registrations[scenarioKey];

                user = registration.Get(virtualUserId);
            }

            return user;
        }

        private static Dictionary<string, List<UsersData>> GetDataInFormatForFile(
            List<Scenario> scenarios, Dictionary<string, ScenarioData> dataForScenariosWithDatabaseIds)
        {
            var scenariosWhichRequireData =
                scenarios.Where(x => x.RequiresData).Select(y => y.GetType().Name.Replace("Scenario", ""))
                    .ToList();

            var allData = new Dictionary<string, List<UsersData>>();

            foreach (var scenarioToWrite in scenariosWhichRequireData)
            {
                var usersDataList = new List<UsersData>();
                foreach (var key in dataForScenariosWithDatabaseIds[scenarioToWrite].Data.Keys)
                {
                   usersDataList.Add(new UsersData
                   {
                       UserId = key,
                       Data = dataForScenariosWithDatabaseIds[scenarioToWrite].Data[key]
                   }); 
                }

                allData.Add(scenarioToWrite, usersDataList);
            }

            return allData;
        }

        private void StoreVirtualUserGlobalIds(Dictionary<string, ScenarioData> data)
        {
            foreach (var scenarioData in data)
                VirtualUserGlobalIds.TryAdd(scenarioData.Key, data[scenarioData.Key].UserGlobalIds);
        }
    }
}