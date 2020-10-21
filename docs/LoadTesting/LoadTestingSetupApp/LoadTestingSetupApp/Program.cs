using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoadTestingSetupApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var testDataSettings = new TestDataSettings();
            configuration.Bind("TestDataSettings", testDataSettings);

            Console.WriteLine($"About to setup test data for scenarios");
            Console.WriteLine($"OrderMobiles: {testDataSettings.OrderMobilesCount}");
            Console.WriteLine($"CompleteProvisionsCount: {testDataSettings.CompleteProvisionsCount}");
            Console.WriteLine($"ActivateMobilesCount: {testDataSettings.ActivateMobilesCount}");
            Console.WriteLine($"CompleteActivatesCount: {testDataSettings.CompleteActivatesCount}");
            Console.WriteLine($"Load test data file to create: {testDataSettings.Path}");

            Console.WriteLine($"Starting...");

            var testDataBuilder = new TestDataBuilder();
            var testData = testDataBuilder.Build(testDataSettings);
            Console.WriteLine($"Database updated...");

            var json = ConvertToJson(testData);

            Console.WriteLine($"Writing to load test data file...");
            File.WriteAllText(testDataSettings.Path, json);

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