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

        public void Write(List<DataForScenario> dataInFormatForFile)
        {
            File.WriteAllText(
                Path.Combine(testDataSettings.Path, 
                    testDataSettings.FileNameScenarios),
                GenerateScenarioText(dataInFormatForFile));
        }

        private string GenerateScenarioText(List<DataForScenario> data)
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append("==================================================");
            builder.AppendLine();

            var orderMobileData = data.FirstOrDefault(x => x.ScenarioName == "OrderMobile");

            if (orderMobileData != null)
            {
                foreach (var usersData in orderMobileData.Data)
                {
                    var iterationData = usersData.Data.First();
                    var scenariosTextTemplate = $@"
--------------------------------------------------
Scenario: OrderMobile
--------------------------------------------------
DATA
CustomerId:         {iterationData["customerId"]}
PhoneNumber:        {iterationData["phoneNumber"]}
ContactName:        {iterationData["contactName"]}
ContactPhoneNumber: {iterationData["contactPhoneNumber"]}
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

            var completeProvisionData = data.FirstOrDefault(x => x.ScenarioName == "CompleteProvision");

            if (completeProvisionData != null)
            {
                foreach (var usersData in completeProvisionData.Data)
                {
                    var iterationData = usersData.Data.First();
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

            var activateMobileData = data.FirstOrDefault(x => x.ScenarioName == "ActivateMobile");

            if (activateMobileData != null)
            {
                foreach (var usersData in activateMobileData.Data)
                {
                    var iterationData = usersData.Data.First();
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

            var completeActivateData = data.FirstOrDefault(x => x.ScenarioName == "CompleteActivate");

            if (completeActivateData != null)
                foreach (var usersData in completeActivateData.Data)
                {
                    var iterationData = usersData.Data.First();
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