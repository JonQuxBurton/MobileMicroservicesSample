using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using EndToEndApiLevelTests.Scenario_Order_a_Mobile;
using EndToEndApiLevelTests.DataAcess;

namespace EndToEndApiLevelTests
{
    public partial class Scenario_Order_a_Mobile_Script
    {
        public Step_1_Snapshot Step_1_Snapshot { get; private set; }
        public Step_2_Snapshot Step_2_Snapshot { get; private set; }
        public Step_3_Snapshot Step_3_Snapshot { get; private set; }
        public Step_4_Snapshot Step_4_Snapshot { get; private set; }
        public Step_5_Snapshot Step_5_Snapshot { get; private set; }

        public Scenario_Order_a_Mobile_Script()
        {
            Execute().Wait();
        }

        private async Task Execute()
        {
            var config = new Config();
            var data = new Data(config);
            var snapshotFactory = new SnapshotFactory(config, data);
            var client = new HttpClient();

            // Step 1 Create a Customer
            var customersUrl = "http://localhost:5000/api/customers";

            var customerToAdd = new MobileOrderer.Api.Resources.CustomerToAdd
            {
                Name = "Armstrong Corporation"
            };

            HttpResponseMessage createCustomerResponse = await client.PostAsJsonAsync(customersUrl, customerToAdd);

            createCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var actualCustomerReturned = await DeserializeResponse<MobileOrderer.Api.Resources.CustomerResource>(createCustomerResponse);

            Step_1_Snapshot = snapshotFactory.Take_Step_1_Snapshot(customerToAdd, actualCustomerReturned);

            // Step 2 Order a Mobile
            var url = $"http://localhost:5000/api/customers/{actualCustomerReturned.GlobalId}/provision";

            var orderToAdd = new MobileOrderer.Api.Resources.OrderToAdd
            {
                PhoneNumber = "0701000001",
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage orderAMobileResponse = await client.PostAsJsonAsync(url, orderToAdd);

            orderAMobileResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Gather needed data
            var actualMobileReturned = await DeserializeResponse<MobileOrderer.Api.Resources.MobileResource>(orderAMobileResponse);
            var actualMobileOrder = data.MobilesData.GetMobileOrder(actualMobileReturned.Id);
            var mobileGlobalId = actualMobileReturned.GlobalId;
            var orderAMobileOrderReference = actualMobileOrder.GlobalId;

            Step_2_Snapshot = snapshotFactory.Take_Step_2_Snapshot(orderToAdd, mobileGlobalId, orderAMobileOrderReference, actualCustomerReturned);

            // Step 3 Complete Mobile Order
            var externalSimCardsProviderUrl = $"http://localhost:5001/api/orders/{orderAMobileOrderReference}/complete";

            HttpResponseMessage actualCompleteOrderResponse = await client.PostAsync(externalSimCardsProviderUrl, null);

            actualCompleteOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            Step_3_Snapshot = snapshotFactory.Take_Step_3_Snapshot(mobileGlobalId, orderAMobileOrderReference);

            // Step 4 Activate a Mobile
            var activateTheMobileUrl = $"http://localhost:5000/api/mobiles/{mobileGlobalId}/activate";
            var activateTheMobileOrder = new MobileOrderer.Api.Resources.OrderToAdd
            {
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage actualActivateTheMobileResponse = await client.PostAsJsonAsync(activateTheMobileUrl, activateTheMobileOrder);

            actualActivateTheMobileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var actualActivateOrderReturned = await DeserializeResponse<MobileOrderer.Api.Resources.OrderResource>(actualActivateTheMobileResponse);
            var activateAMobileOrderReference = actualActivateOrderReturned.GlobalId;

            Step_4_Snapshot = snapshotFactory.Take_Step_4_Snapshot(mobileGlobalId, activateAMobileOrderReference);

            // Step 5 Complete Activate Order
            var externalMobileTelecomsNetworkUrl = $"http://localhost:5002/api/orders/{activateAMobileOrderReference}/complete";

            HttpResponseMessage actualCompleteActivateOrderResponse = await client.PostAsync(externalMobileTelecomsNetworkUrl, null);

            actualCompleteActivateOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Step 4 Snapshot
            Step_5_Snapshot = snapshotFactory.Take_Step_5_Snapshot(mobileGlobalId, activateAMobileOrderReference);
        }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            var deserialized = JsonSerializer.Deserialize<T>(stringResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return deserialized;
        }
    }
}
