using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SimCards.EventHandlers.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimCards.EventHandlers.Tests.Services
{
    public class SimCardWholesaleServiceSpec
    {
        public class PostOrderShould
        {
            private Config config;
            private IOptions<Config> options;
            private Uri expectedUrl;
            private SimCardWholesalerOrder expectedOrder;
            private HttpClient httpClient;
            private Mock<DelegatingHandler> handlerMock;

            public PostOrderShould()
            {
                config = new Config
                {
                    SimCardWholesalerApiUrl = "http://api:5000"
                };
                options = Options.Create(config);
                expectedUrl = new Uri($"{config.SimCardWholesalerApiUrl}/api/orders");
                expectedOrder = new SimCardWholesalerOrder { 
                    Name = "Neil Armstrong",
                    Reference = Guid.NewGuid()
                };
                handlerMock = new Mock<DelegatingHandler>();
                handlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.Is<HttpRequestMessage>(
                            x => x.RequestUri == expectedUrl),
                        ItExpr.IsAny<CancellationToken>())
                    .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonSerializer.Serialize(expectedOrder))
                    }
                    ));
                httpClient = new HttpClient(handlerMock.Object);


            }

            [Fact]
            public async void CallExternalService()
            {
                var sut = new SimCardWholesaleService(options, httpClient);

                await sut.PostOrder(expectedOrder);

                handlerMock.VerifyAll();
            }

            [Fact]
            public async void ReturnTrue()
            {
                var sut = new SimCardWholesaleService(options, httpClient);

                var actual = await sut.PostOrder(expectedOrder);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void ReturnFalseWhenResponseStatusCodeIsNotOK()
            {
                var handlerWithErrorMock = new Mock<DelegatingHandler>();
                handlerWithErrorMock.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.Is<HttpRequestMessage>(
                            x => x.RequestUri == expectedUrl),
                        ItExpr.IsAny<CancellationToken>())
                    .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(JsonSerializer.Serialize(expectedOrder))
                    }
                    ));
                var httpClientWithError = new HttpClient(handlerWithErrorMock.Object);

                var sut = new SimCardWholesaleService(options, httpClientWithError);

                var actual = await sut.PostOrder(expectedOrder);

                actual.Should().BeFalse();
            }
        }
    }
}
