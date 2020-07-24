using FluentAssertions;
using Microsoft.Extensions.Options;
using MobileTelecomsNetwork.EventHandlers.Domain;
using MobileTelecomsNetwork.EventHandlers.Tests.BackgroundServices;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MobileTelecomsNetwork.EventHandlers.Tests.Domain
{
    public class ExternalMobileTelecomsNetworkServiceSpec
    {
        private Config config;
        private ExternalMobileTelecomsNetworkOrder expectedOrder;
        private IOptions<Config> options;
        private Uri expectedUrl;

        public ExternalMobileTelecomsNetworkServiceSpec()
        {
            config = new Config
            {
                ExternalMobileTelecomsNetworkApiUrl = "http://www.api.com"
            };
            expectedOrder = new ExternalMobileTelecomsNetworkOrder();
            options = Options.Create(config);
            expectedUrl = new Uri($"{config.ExternalMobileTelecomsNetworkApiUrl}/api/orders");
        }

        [Fact]
        public async void PostOrderShouldReturn()
        {
            var clientHandlerStub = new DelegatingHandlerStub();
            var client = new HttpClient(clientHandlerStub);

            var sut = new ExternalMobileTelecomsNetworkService(options, client);

            var actual = await sut.PostOrder(expectedOrder);

            actual.Should().BeTrue();
        }

        [Fact]
        public async void PostOrderShouldPostOrderToExternalService()
        {
            var handlerMock = new Mock<DelegatingHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        x => x.RequestUri == expectedUrl && HttpContentEquals(x.Content, expectedOrder)),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
            var client = new HttpClient(handlerMock.Object);

            var sut = new ExternalMobileTelecomsNetworkService(options, client);

            await sut.PostOrder(expectedOrder);

            handlerMock.VerifyAll();
        }

        [Fact]
        public async void ReturnFalseWhenPostOrderFails()
        {
            var clientHandlerStub = new DelegatingHandlerStub((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            ));
            var client = new HttpClient(clientHandlerStub);
            var sut = new ExternalMobileTelecomsNetworkService(options, client);

            var actual = await sut.PostOrder(expectedOrder);

            actual.Should().BeFalse();
        }

        private bool HttpContentEquals(HttpContent content, ExternalMobileTelecomsNetworkOrder order)
        {
            var json = JsonConvert.SerializeObject(order);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            return content.ReadAsStringAsync().Result == stringContent.ReadAsStringAsync().Result;
        }
    }
}
