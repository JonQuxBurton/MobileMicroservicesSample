using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoadTestingSetupApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new Config
            {
                Path = @"D:\Projects\GitHub\MobileMicroservicesSample\docs\LoadTesting\data.json",
                ConnectionString = "Server=JQB1-2020;Initial Catalog=Mobile;Integrated Security=True",
                CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F",
                OrderMobilesCount = 3,
                CompleteProvisionsCount = 3,
                ActivateMobilesCount = 3,
                CompleteActivatesCount = 3
            };

            Console.WriteLine($"About to setup test data for scenarios");
            Console.WriteLine($"OrderMobiles: {config.OrderMobilesCount}");
            Console.WriteLine($"CompleteProvisionsCount: {config.CompleteProvisionsCount}");
            Console.WriteLine($"ActivateMobilesCount: {config.ActivateMobilesCount}");
            Console.WriteLine($"CompleteActivatesCount: {config.CompleteActivatesCount}");
            Console.WriteLine($"Load test data file to create: {config.Path}");

            Console.WriteLine($"Starting...");

            var testDataBuilder = new TestDataBuilder();
            var testData = testDataBuilder.Build(config);
            Console.WriteLine($"Database updated...");

            var json = ConvertToJson(testData);

            Console.WriteLine($"Writing to load test data file...");
            File.WriteAllText(config.Path, json);

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
    }
}