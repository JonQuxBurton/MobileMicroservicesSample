using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SimCards.EventHandlers.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq.Protected;
using Newtonsoft.Json;

namespace SimCards.EventHandlers.Tests.Domain
{
    public class SimCardWholesalerServiceSpec
    {
        public class PostOrderShould
        {
            [Fact]
            public async void ReturnTrueWhenPostOrderSucceeds()
            {
                var clientHandlerStub = new DelegatingHandlerStub();
                var client = new HttpClient(clientHandlerStub);
                var options = Options.Create<Config>(
                    new Config
                    {
                        SimCardWholesalerApiUrl = "http://www.api.com"
                    });
                var sut = new SimCardWholesaleService(options, client);

                var actual = await sut.PostOrder(new SimCardWholesalerOrder { });

                actual.Should().BeTrue();
            }

            [Fact]
            public async void PostOrder()
            {
                var config = new Config
                {
                    SimCardWholesalerApiUrl = "http://www.api.com"
                };
                var expectedOrder = new SimCardWholesalerOrder {
                    Name = "Neil Armstrong",
                    Reference = Guid.NewGuid()
                };
                var expectedUrl = new Uri($"{config.SimCardWholesalerApiUrl}/api/orders");
                var json = JsonConvert.SerializeObject(expectedOrder);
                var options = Options.Create<Config>(config);
                var handlerMock = new Mock<DelegatingHandler>();
                handlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.Is<HttpRequestMessage>(
                            x => x.RequestUri == expectedUrl && HttpContentEquals(x.Content, expectedOrder)),
                        ItExpr.IsAny<CancellationToken>())
                    .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
                var client = new HttpClient(handlerMock.Object);

                var sut = new SimCardWholesaleService(options, client);

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
                var options = Options.Create<Config>(
                    new Config
                    {
                        SimCardWholesalerApiUrl = "http://www.api.com"
                    });
                var sut = new SimCardWholesaleService(options, client);

                var actual = await sut.PostOrder(new SimCardWholesalerOrder { });

                actual.Should().BeFalse();
            }

            private bool HttpContentEquals(HttpContent content, SimCardWholesalerOrder order)
            {
                var json = JsonConvert.SerializeObject(order);
                var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                return content.ReadAsStringAsync().Result == stringContent.ReadAsStringAsync().Result;
            }
        }
    }
}
