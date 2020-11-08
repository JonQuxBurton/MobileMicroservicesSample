using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;

namespace LoadTestingWebService
{
    public class ScenarioScriptFileWriter : IScenarioScriptFileWriter
    {
        private readonly TestDataSettings testDataSettings;

        public ScenarioScriptFileWriter(IOptions<TestDataSettings> testDataSettingsOptions)
        {
            testDataSettings = testDataSettingsOptions.Value;
        }

        public void Write(Dictionary<string, List<UsersData>>  dataInFormatForFile)
        {
            File.WriteAllText(
                Path.Combine(testDataSettings.Path, 
                    testDataSettings.FileNameScenarios),
                GenerateScenarioText(dataInFormatForFile));
        }

        private string GenerateScenarioText(Dictionary<string, List<UsersData>> data)
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append("==================================================");
            builder.AppendLine();

            if (data["OrderMobile"].Any())
            {
                foreach (var usersData in data["OrderMobile"])
                {
                    var iterationsData = usersData.Data[0];
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: OrderMobile
--------------------------------------------------
DATA
CustomerId:         {iterationsData["customerId"]}
PhoneNumber:        {iterationsData["phoneNumber"]}
ContactName:        {iterationsData["contactName"]}
ContactPhoneNumber: {iterationsData["contactPhoneNumber"]}
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

            if (data["CompleteProvision"].Any())
            {
                foreach (var usersData in data["CompleteProvision"])
                {
                    var iterationData = usersData.Data[0];
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: CompleteProvision
--------------------------------------------------
DATA
CustomerId:         {iterationData["customerId"]}
PhoneNumber:        {iterationData["phoneNumber"]}
MobileId:           {iterationData["mobileId"]}
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

            if (data["ActivateMobile"].Any())
            {
                foreach (var usersData in data["ActivateMobile"])
                {
                    var iterationData = usersData.Data[0];
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: ActivateMobile
--------------------------------------------------
DATA
CustomerId:     {iterationData["customerId"]}
PhoneNumber:    {iterationData["phoneNumber"]}
MobileId:       {iterationData["mobileId"]}
ActivationCode: {iterationData["activationCode"]}
--------------------------------------------------
AT START
Mobile has
    State:      'WaitingForActivate'
--------------------------------------------------
AT END
Mobile updated to
    State:      'ProcessingActivate'
MobileTelecomsNetwork Order created with
    MobileId    {iterationData["mobileDbId"]}
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

            if (data["CompleteActivate"].Any())
                foreach (var usersData in data["CompleteActivate"])
                {
                    var iterationData = usersData.Data[0];
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: CompleteActivate
--------------------------------------------------
DATA
CustomerId:     {iterationData["customerId"]}
PhoneNumber:    {iterationData["phoneNumber"]}
MobileId:       {iterationData["mobileId"]}
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
    MobileId    {iterationData["mobileDbId"]}
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
}