using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;

namespace LoadTestingWebService
{
    public class ScenarioTextGenerator : IScenarioTextGenerator
    {
        private readonly TestDataSettings testDataSettings;

        public ScenarioTextGenerator(IOptions<TestDataSettings> testDataSettingsOptions)
        {
            testDataSettings = testDataSettingsOptions.Value;
        }

        public string GenerateScenarioText(Data data)
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append("==================================================");
            builder.AppendLine();

            if (data.OrderMobile.Any())
            {
                foreach (var orderMobileTestData in data.OrderMobile.Values.ElementAt(0))
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
                foreach (var orderMobileTestData in data.CompleteProvision.Values.ElementAt(0))
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
                foreach (var activateMobileTestData in data.ActivateMobile.Values.ElementAt(0))
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
                foreach (var completeActivateTestData in data.CompleteActivate.Values.ElementAt(0))
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
}