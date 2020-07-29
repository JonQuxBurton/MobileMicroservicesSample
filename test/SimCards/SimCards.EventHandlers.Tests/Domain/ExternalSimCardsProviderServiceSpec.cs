using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
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
    public static class ExternalSimCardsProviderServiceSpec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public class PostOrderShould
        {
            private readonly Config config;
            private readonly IOptions<Config> options;
            private readonly Uri expectedUrl;
            private readonly ExternalSimCardOrder expectedOrder;
            private readonly HttpClient httpClient;
            private readonly Mock<DelegatingHandler> handlerMock;

            public PostOrderShould()
            {
                config = new Config
                {
                    ExternalSimCardsProviderApiUrl = "http://api:5000"
                };
                options = Options.Create(config);
                expectedUrl = new Uri($"{config.ExternalSimCardsProviderApiUrl}/api/orders");
                expectedOrder = new ExternalSimCardOrder
                {
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
                var sut = new ExternalSimCardsProviderService(options, httpClient);

                await sut.PostOrder(expectedOrder);

                handlerMock.VerifyAll();
            }

            [Fact]
            public async void ReturnTrue()
            {
                var sut = new ExternalSimCardsProviderService(options, httpClient);

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

                var sut = new ExternalSimCardsProviderService(options, httpClientWithError);

                var actual = await sut.PostOrder(expectedOrder);

                actual.Should().BeFalse();
            }
        }
    }
}
