using FluentAssertions;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Messages;
using Mobiles.Api.Services;
using Moq;
using Utils.DomainDrivenDesign;
using Xunit;

namespace Mobiles.Api.Tests.Services
{
    namespace ProcessingProvisionEventCheckerSpec
    {
        public class CheckShould
        {
            private readonly Mobile expectedMobile;
            private readonly ProcessingProvisionEventChecker sut;
            private readonly Mock<IGetProcessingProvisionMobilesQuery> queryMock;
            private readonly Mock<IRepository<Mobile>> repositoryMock;
            private readonly Mock<IMessagePublisher> messagePublisherMock;

            public CheckShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = Mobile.MobileState.ProcessingActivate.ToString()
                }, new Order(new OrderDataEntity
                {
                    State = Order.State.New.ToString()
                }));

                queryMock = new Mock<IGetProcessingProvisionMobilesQuery>();
                queryMock.Setup(x => x.Get())
                    .Returns(new[] { expectedMobile });
                repositoryMock = new Mock<IRepository<Mobile>>();
                messagePublisherMock = new Mock<IMessagePublisher>();
                var loggerMock = new Mock<ILogger<ProcessingProvisionEventChecker>>();

                sut = new ProcessingProvisionEventChecker(loggerMock.Object, queryMock.Object, repositoryMock.Object, messagePublisherMock.Object);
            }

            [Fact]
            public void SetTheMobileToProcessingActivate()
            {
                sut.Check();

                expectedMobile.State.Should().Be(Mobile.MobileState.ProcessingActivate);
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
            public void PublishMobileRequested()
            {
                sut.Check();

                messagePublisherMock.Verify(x => x.PublishAsync(It.Is<ProvisionRequestedMessage>(
                    y => y.MobileOrderId == expectedMobile.InFlightOrder.GlobalId &&
                            y.Name == expectedMobile.InFlightOrder.Name &&
                            y.ContactPhoneNumber == expectedMobile.InFlightOrder.ContactPhoneNumber)));
            }
        }
    }
}
