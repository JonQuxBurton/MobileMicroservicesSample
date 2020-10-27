using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoadTestingWebService
{
    public class SetupData
    {
        private readonly TestDataSettings testDataSettings;

        public SetupData(TestDataSettings testDataSettings)
        {
            this.testDataSettings = testDataSettings;
        }

        public void Execute()
        {
            Console.WriteLine("About to setup test data for scenarios");
            Console.WriteLine($"OrderMobiles - VUs: {testDataSettings.OrderMobilesSettings.VirtualUsers}, Iterations: {testDataSettings.OrderMobilesSettings.Iterations}");
            Console.WriteLine($"CompleteProvisionsSettings - VUs: {testDataSettings.CompleteProvisionsSettings.VirtualUsers}, Iterations: {testDataSettings.CompleteProvisionsSettings.Iterations}");
            Console.WriteLine($"ActivateMobilesSettings - VUs: {testDataSettings.ActivateMobilesSettings.VirtualUsers}, Iterations: {testDataSettings.ActivateMobilesSettings.Iterations}");
            Console.WriteLine($"CompleteActivatesSettings - VUs: {testDataSettings.CompleteActivatesSettings.VirtualUsers}, Iterations: {testDataSettings.CompleteActivatesSettings.Iterations}");
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