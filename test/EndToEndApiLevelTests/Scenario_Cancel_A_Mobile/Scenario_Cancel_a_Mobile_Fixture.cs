using FluentAssertions;
using System.Net;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace EndToEndApiLevelTests.Scenario_Cancel_A_Mobile
{
    public class Scenario_Cancel_a_Mobile_Fixture : IDisposable
    {
        public Step_1_Snapshot Step_1_Snapshot { get; private set; }
        public Step_2_Snapshot Step_2_Snapshot { get; private set; }

        public Scenario_Cancel_a_Mobile_Fixture()
        {
            Execute().Wait();
        }

        private async Task Execute()
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";
            var mobilesData = new MobilesData(connectionString);

            var snapshotFactory = new SnapshotFactory(connectionString, TimeSpan.FromSeconds(10));

            var reference = Guid.NewGuid();
            var mobileWhichIsLive = mobilesData.CreateMobile(reference, "Live");

            // Step 1 - Cancel a Mobile
            var client = new HttpClient();            
            var url = $"http://localhost:5000/api/mobiles/{reference}";
            HttpResponseMessage cancelAMobileResponse = await client.DeleteAsync(url);

            cancelAMobileResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

            // Gather needed data
            var actualMobile = mobilesData.GetMobile(mobileWhichIsLive.GlobalId);
            var actualMobileOrder = mobilesData.GetMobileOrder(actualMobile.Id);
            var orderReference = actualMobileOrder.GlobalId;

            Step_1_Snapshot = snapshotFactory.Take_Step_1_Snapshot(actualMobile);

            // Step 2 - Cancel Order has been Completed
            var completeOrderUrl = $"http://localhost:5002/api/orders/{orderReference}/complete";
            HttpResponseMessage completeOrderResponse = await client.PostAsync(completeOrderUrl, null);

            completeOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            Step_2_Snapshot = snapshotFactory.Take_Step_2_Snapshot(actualMobile);
        }

        public void Dispose()
        {

        }

    }
}
