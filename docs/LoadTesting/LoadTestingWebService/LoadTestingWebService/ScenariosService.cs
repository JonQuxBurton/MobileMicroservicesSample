using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LoadTestingWebService.Data;
using LoadTestingWebService.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoadTestingWebService
{
    public class ScenariosService : IScenariosService
    {
        private readonly TestDataSettings testDataSettings;
        private readonly IScenarioTextGenerator scenarioTextGenerator;
        private readonly IDataStore dataStore;
        public static int GlobalCounter = 1;

        private readonly object locker = new object();

        public ConcurrentDictionary<string, VirtualUserRegistrations> Registrations =
            new ConcurrentDictionary<string, VirtualUserRegistrations>();

        public ConcurrentDictionary<string, List<Guid>> VirtualUserGlobalIds =
            new ConcurrentDictionary<string, List<Guid>>();

        public ScenariosService(IOptions<TestDataSettings> testDataSettingsOptions,
            IScenarioTextGenerator scenarioTextGenerator,
            IDataStore dataStore)
        {
            this.testDataSettings = testDataSettingsOptions.Value;
            this.scenarioTextGenerator = scenarioTextGenerator;
            this.dataStore = dataStore;
        }

        public void GenerateData()
        {
            Console.WriteLine("Generating data...");
            var createCustomersUsers = new List<Guid>();
            for (var i = 0; i < testDataSettings.CreateCustomersSettings.VirtualUsers; i++)
                createCustomersUsers.Add(Guid.NewGuid());
            VirtualUserGlobalIds.TryAdd("createCustomer", createCustomersUsers);

            var orderMobilesBuilder = new OrderMobilesBuilder();
            var dataForOrderMobiles = orderMobilesBuilder.Build(testDataSettings);
            var users = new List<Guid>();
            for (var i = 0; i < dataForOrderMobiles.Keys.Count; i++)
                users.Add(dataForOrderMobiles.Keys.ElementAt(i));
            VirtualUserGlobalIds.TryAdd("orderMobile", users);

            var completeProvisionsBuilder = new CompleteProvisionsBuilder();
            var dataForCompleteProvisions = completeProvisionsBuilder.Build(testDataSettings);
            var users2 = new List<Guid>();
            for (var i = 0; i < dataForCompleteProvisions.Keys.Count; i++)
                users2.Add(dataForCompleteProvisions.Keys.ElementAt(i));
            VirtualUserGlobalIds.TryAdd("completeProvision", users2);

            var activateMobilesBuilder = new ActivateMobilesBuilder();
            var dataForActivateMobiles = activateMobilesBuilder.Build(testDataSettings);
            var users3 = new List<Guid>();
            for (var i = 0; i < dataForActivateMobiles.Keys.Count; i++)
                users3.Add(dataForActivateMobiles.Keys.ElementAt(i));
            VirtualUserGlobalIds.TryAdd("activateMobile", users3);

            var completeActivatesBuilder = new CompleteActivatesBuilder();
            var dataForCompleteActivates = completeActivatesBuilder.Build(testDataSettings);
            var users4 = new List<Guid>();
            for (var i = 0; i < dataForCompleteActivates.Keys.Count; i++)
                users4.Add(dataForCompleteActivates.Keys.ElementAt(i));
            VirtualUserGlobalIds.TryAdd("completeActivate", users4);

            Console.WriteLine("Writing data to Database...");
            var completeProvisionsDataWriter = new CompleteProvisionsDataWriter();
            completeProvisionsDataWriter.Write(dataStore, dataForCompleteProvisions);

            var activateMobilesDataWriter = new ActivateMobilesDataWriter();
            activateMobilesDataWriter.Write(dataStore, dataForActivateMobiles);

            var completeActivatesDataWriter = new CompleteActivatesDataWriter();
            completeActivatesDataWriter.Write(dataStore, dataForCompleteActivates);

            Console.WriteLine($"Writing data to load testing data file ({testDataSettings.FileNameData})...");
            var allData = new DataHolder
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
                this.scenarioTextGenerator.GenerateScenarioText(allData));

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

        private static string ConvertToJson(DataHolder data)
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
    }
}