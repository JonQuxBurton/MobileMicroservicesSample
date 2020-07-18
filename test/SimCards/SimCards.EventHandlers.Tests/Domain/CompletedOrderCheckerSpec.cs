using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalEventBus.JustSaying;
using Moq;
using Moq.Protected;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Domain;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimCards.EventHandlers.Tests.Domain
{
    public class CompletedOrderCheckerSpec
    {
        [Fact]
        public async void CallExternalService()
        {
            var config = new Config
            {
                SimCardWholesalerApiUrl = "http://api:5000"
            };
            var expectedOrder = new SimCardOrder()
            {
                MobileOrderId = Guid.NewGuid()
            };
            var expectedUrl = new Uri($"{config.SimCardWholesalerApiUrl}/api/orders/{expectedOrder.MobileOrderId}");
            var externalOrder = new SimCardOrder
            {
                Name = "Neil Armstrong",
                Status = "Completed"
            };
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var dataStoreMock = new Mock<ISimCardOrdersDataStore>();
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

            var sut = new CompletedOrderChecker(Mock.Of<ILogger<CompletedOrderChecker>>(),
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
