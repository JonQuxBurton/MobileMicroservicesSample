using FluentAssertions;
using System.Net;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using EndToEndApiLevelTests.DataAcess;
using Mobiles.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Cease_a_Mobile
{
    public class Scenario_Cease_a_Mobile_Script
    {
        public Step_1_Snapshot Step_1_Snapshot { get; private set; }
        public Step_2_Snapshot Step_2_Snapshot { get; private set; }

        public Scenario_Cease_a_Mobile_Script()
        {
            Execute().Wait();
        }

        private async Task Execute()
        {
            var config = new Config();
            var data = new Data(config);
            var snapshotFactory = new SnapshotFactory(config, data);

            var phoneNumber = new PhoneNumber("09001000001");
            var mobileGlobalId = Guid.NewGuid();
            data.MobilesData.CreateMobile(phoneNumber, mobileGlobalId, "Live");
            var mobileWhichIsLive = data.MobilesData.GetMobileByGlobalId(mobileGlobalId);

            // Step 1 - Cease a Mobile
            var client = new HttpClient();
            var url = $"http://localhost:5000/api/mobiles/{mobileGlobalId}";
            HttpResponseMessage ceaseAMobileResponse = await client.DeleteAsync(url);

            ceaseAMobileResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

            // Gather needed data
            var actualMobile = data.MobilesData.GetMobileByGlobalId(mobileWhichIsLive.GlobalId);
            var actualMobileOrder = data.MobilesData.GetMobileOrder(actualMobile.Id);
            var orderReference = actualMobileOrder.GlobalId;

            Step_1_Snapshot = snapshotFactory.Take_Step_1_Snapshot(actualMobile);

            // Step 2 - Cease Order has been Completed
            var completeOrderUrl = $"http://localhost:5002/api/orders/{orderReference}/complete";
            HttpResponseMessage completeOrderResponse = await client.PostAsync(completeOrderUrl, null);

            completeOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            Step_2_Snapshot = snapshotFactory.Take_Step_2_Snapshot(actualMobile);
        }
    }
}
