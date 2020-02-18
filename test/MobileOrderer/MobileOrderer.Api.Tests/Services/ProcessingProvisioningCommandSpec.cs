using FluentAssertions;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using MobileOrderer.Api.Services;
using Moq;
using Utils.DomainDrivenDesign;
using Utils.Enums;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class ProcessingProvisioningCommandSpec
    {
        public class ExecuteShould
        {
            private Mobile expectedMobile;
            private Mock<IRepository<Mobile>> repositoryMock;
            private Mock<IMessagePublisher>messagePublisherMock;
            private ProcessingProvisioningCommand sut;

            public ExecuteShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = new EnumConverter().ToName<Mobile.State>(Mobile.State.ProcessingActivation)
                }, new Order(new OrderDataEntity { 
                    State = new EnumConverter().ToName<Order.State>(Order.State.New)
                }));
                repositoryMock = new Mock<IRepository<Mobile>>();
                messagePublisherMock = new Mock<IMessagePublisher>();
                sut = new ProcessingProvisioningCommand(repositoryMock.Object, messagePublisherMock.Object);
            }

            [Fact]
            public void SetsTheMobileToOrderProcessing()
            {
                sut.Execute(expectedMobile);

                expectedMobile.CurrentState.Should().Be(Mobile.State.ProcessingActivation);
                expectedMobile.InFlightOrder.CurrentState.Should().Be(Order.State.Processing);
            }

            [Fact]
            public void UpdateTheRepository()
            {
                sut.Execute(expectedMobile);

                repositoryMock.Verify(x => x.Update(expectedMobile));
            }

            [Fact]
            public void PublishesMobileRequested()
            {
                sut.Execute(expectedMobile);

                messagePublisherMock.Verify(x => x.PublishAsync(It.Is<MobileRequestedMessage>(
                    y => y.MobileOrderId == expectedMobile.InFlightOrder.GlobalId &&
                            y.Name == expectedMobile.InFlightOrder.Name &&
                            y.ContactPhoneNumber == expectedMobile.InFlightOrder.ContactPhoneNumber)));
            }
        }
    }
}
