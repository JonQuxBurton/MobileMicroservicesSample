using FluentAssertions;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using MobileOrderer.Api.Services;
using Moq;
using Utils.DomainDrivenDesign;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public static class CeaseRequestedEventCheckerSpec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public class CheckShould
        {
            private readonly Mobile expectedMobile;
            private readonly CeaseRequestedEventChecker sut;
            private readonly Mock<IGetNewCeasesQuery> getMobilesQueryMock;
            private readonly Mock<IRepository<Mobile>> repositoryMock;
            private readonly Mock<IMessagePublisher> messagePublisherMock;

            public CheckShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = Mobile.State.ProcessingCease.ToString()
                }, new Order(new OrderDataEntity
                {
                    State = Order.State.New.ToString()
                }));
                repositoryMock = new Mock<IRepository<Mobile>>();
                messagePublisherMock = new Mock<IMessagePublisher>();
                getMobilesQueryMock = new Mock<IGetNewCeasesQuery>();
                getMobilesQueryMock.Setup(x => x.Get())
                    .Returns(new[] { expectedMobile });
                var loggerMock = new Mock<ILogger<CeaseRequestedEventChecker>>();

                sut = new CeaseRequestedEventChecker(loggerMock.Object, getMobilesQueryMock.Object,
                    repositoryMock.Object,
                    messagePublisherMock.Object);
            }

            [Fact]
            public void SetTheMobileToProcessingCease()
            {
                sut.Check();

                expectedMobile.CurrentState.Should().Be(Mobile.State.ProcessingCease);
            }

            [Fact]
            public void SetTheOrderToProcessing()
            {
                sut.Check();

                expectedMobile.InFlightOrder.CurrentState.Should().Be(Order.State.Processing);
            }

            [Fact]
            public void UpdateTheRepository()
            {
                sut.Check();

                repositoryMock.Verify(x => x.Update(expectedMobile));
            }

            [Fact]
            public void PublishCeaseRequestedMessage()
            {
                sut.Check();

                messagePublisherMock.Verify(x => x.PublishAsync(It.Is<CeaseRequestedMessage>(
                    y => y.MobileOrderId == expectedMobile.InFlightOrder.GlobalId)));
            }
        }
    }
}
