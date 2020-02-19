using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Services;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MobileTelecomsNetwork.EventHandlers.Tests.Services
{
    public class ActivationOrderCheckerSpec
    {
        [Fact]
        public async void CallExternalService()
        {
            var config = new Config
            {
                ExternalMobileTelecomsNetworkApiUrl = "http://api:5000"
            };
            var expectedOrder = new ActivationOrder()
            {
                MobileOrderId = Guid.NewGuid()
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

            var sut = new ActivationOrderChecker(Mock.Of<ILogger<CompletedOrderPollingHostedService>>(),
                httpClientFactoryMock.Object,
                dataStoreMock.Object,
                messagePublisherMock.Object,
                options);

            await sut.Check(expectedOrder);

            handlerMock.VerifyAll();
        }
    }
}
