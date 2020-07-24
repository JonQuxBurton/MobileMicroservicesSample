using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.BackgroundServices;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MobileTelecomsNetwork.EventHandlers.Tests.Domain
{
    public class OrderCompletedCheckerSpec
    {
        [Theory]
        [InlineData("Provision")]
        [InlineData("Cease")]
        public async void CallExternalService(string orderType)
        {
            var config = new Config
            {
                ExternalMobileTelecomsNetworkApiUrl = "http://api:5000"
            };
            var expectedOrder = new Order()
            {
                MobileOrderId = Guid.NewGuid(),
                Type = orderType
            };
            var expectedUrl = new Uri($"{config.ExternalMobileTelecomsNetworkApiUrl}/api/orders/{expectedOrder.MobileOrderId}");
            var externalOrder = new ExternalOrder
            {
                Name = "Neil Armstrong",
                Status = "Completed"
            };
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var dataStoreMock = new Mock<IDataStore>();
            var messagePublisherMock = new Mock<IMessagePublisher>();
            var monitoringMock = new Mock<IMonitoring>();
            var options = Options.Create(config);
            var handlerMock = new Mock<DelegatingHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        x => x.RequestUri == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(externalOrder))
                }
                ));
            var client = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(client);

            var sut = new OrderCompletedChecker(Mock.Of<ILogger<CompletedOrderPollingHostedService>>(),
                httpClientFactoryMock.Object,
                dataStoreMock.Object,
                messagePublisherMock.Object,
                options,
                monitoringMock.Object);

            await sut.Check(expectedOrder);

            handlerMock.VerifyAll();
        }
    }
}
