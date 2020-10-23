using System;
using System.IO;
using System.Text;
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
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var testDataSettings = new TestDataSettings();
            configuration.Bind("TestDataSettings", testDataSettings);

            Console.WriteLine("About to setup test data for scenarios");
            Console.WriteLine($"OrderMobiles: {testDataSettings.OrderMobilesCount}");
            Console.WriteLine($"CompleteProvisionsCount: {testDataSettings.CompleteProvisionsCount}");
            Console.WriteLine($"ActivateMobilesCount: {testDataSettings.ActivateMobilesCount}");
            Console.WriteLine($"CompleteActivatesCount: {testDataSettings.CompleteActivatesCount}");
            Console.WriteLine($"Load test data file to create: {testDataSettings.Path}");

            Console.WriteLine("Starting...");

            var testDataBuilder = new TestDataBuilder();
            var testData = testDataBuilder.Build(testDataSettings);
            Console.WriteLine("Database updated...");

            var json = ConvertToJson(testData);

            Console.WriteLine("Writing to load test data file...");
            File.WriteAllText(Path.Combine(testDataSettings.Path, testDataSettings.FileNameData), json);

            Console.WriteLine("Writing scenarios file...");

            File.WriteAllText(
                Path.Combine(testDataSettings.Path, testDataSettings.FileNameScenarios),
                GenerateScenarioText(testData));

            Console.WriteLine("Done!");
        }

        private static string GenerateScenarioText(Data data)
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append("==================================================");
            builder.AppendLine();

            foreach (var orderMobileTestData in data.OrderMobile)
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

            foreach (var orderMobileTestData in data.CompleteProvision)
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

            foreach (var activateMobileTestData in data.ActivateMobile)
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

            foreach (var completeActivateTestData in data.CompleteActivate)
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