using FluentAssertions;
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
    public static class ProcessingProvisioningEventCheckerSpec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public class CheckShould
        {
            private readonly Mobile expectedMobile;
            private readonly ProcessingProvisioningEventChecker sut;
            private readonly Mock<IGetProcessingProvisioningMobilesQuery> queryMock;
            private Mock<IRepository<Mobile>> repositoryMock;
            private Mock<IMessagePublisher> messagePublisherMock;

            public CheckShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = Mobile.State.ProcessingActivation.ToString()
                }, new Order(new OrderDataEntity
                {
                    State = Order.State.New.ToString()
                }));

                queryMock = new Mock<IGetProcessingProvisioningMobilesQuery>();
                queryMock.Setup(x => x.Get())
                    .Returns(new[] { expectedMobile });
                repositoryMock = new Mock<IRepository<Mobile>>();
                messagePublisherMock = new Mock<IMessagePublisher>();

                sut = new ProcessingProvisioningEventChecker(queryMock.Object, repositoryMock.Object, messagePublisherMock.Object);
            }

            [Fact]
            public void SetTheMobileToProcessingActivation()
            {
                sut.Check();

                expectedMobile.CurrentState.Should().Be(Mobile.State.ProcessingActivation);
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

                messagePublisherMock.Verify(x => x.PublishAsync(It.Is<MobileRequestedMessage>(
                    y => y.MobileOrderId == expectedMobile.InFlightOrder.GlobalId &&
                            y.Name == expectedMobile.InFlightOrder.Name &&
                            y.ContactPhoneNumber == expectedMobile.InFlightOrder.ContactPhoneNumber)));
            }
        }
    }
}
