﻿using System.Linq;
using System.Text;

namespace LoadTestingWebService
{
    public class ScenarioTextGenerator : IScenarioTextGenerator
    {
        public string GenerateScenarioText(DataHolder data)
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
CustomerId:         {orderMobileTestData["customerId"]}
PhoneNumber:        {orderMobileTestData["phoneNumber"]}
ContactName:        {orderMobileTestData["contactName"]}
ContactPhoneNumber: {orderMobileTestData["contactPhoneNumber"]}
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
CustomerId:         {orderMobileTestData["customerId"]}
PhoneNumber:        {orderMobileTestData["phoneNumber"]}
MobileId:           {orderMobileTestData["mobileId"]}
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
CustomerId:     {activateMobileTestData["customerId"]}
PhoneNumber:    {activateMobileTestData["phoneNumber"]}
MobileId:       {activateMobileTestData["mobileId"]}
ActivationCode: {activateMobileTestData["activationCode"]}
--------------------------------------------------
AT START
Mobile has
    State:      'WaitingForActivate'
--------------------------------------------------
AT END
Mobile updated to
    State:      'ProcessingActivate'
MobileTelecomsNetwork Order created with
    MobileId    {activateMobileTestData["mobileDbId"]}
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
CustomerId:     {completeActivateTestData["customerId"]}
PhoneNumber:    {completeActivateTestData["phoneNumber"]}
MobileId:       {completeActivateTestData["mobileId"]}
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
    MobileId    {completeActivateTestData["mobileDbId"]}
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