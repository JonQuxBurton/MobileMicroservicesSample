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
    public class CeaseCommandSpec
    {
        public class ExecuteShould
        {
            private readonly Mobile expectedMobile;
            private readonly Mock<IRepository<Mobile>> repositoryMock;
            private Mock<IMessagePublisher> messagePublisherMock;
            private readonly CeaseCommand sut;

            public ExecuteShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = new EnumConverter().ToName<Mobile.State>(Mobile.State.ProcessingCease)
                }, new Order(new OrderDataEntity
                {
                    State = new EnumConverter().ToName<Order.State>(Order.State.New)
                }));
                repositoryMock = new Mock<IRepository<Mobile>>();
                messagePublisherMock = new Mock<IMessagePublisher>();
                sut = new CeaseCommand(repositoryMock.Object, messagePublisherMock.Object);
            }

            [Fact]
            public void SetsTheMobileToOrderProcessing()
            {
                sut.Execute(expectedMobile);

                expectedMobile.CurrentState.Should().Be(Mobile.State.ProcessingCease);
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

                messagePublisherMock.Verify(x => x.PublishAsync(It.Is<CeaselRequestedMessage>(
                    y => y.MobileOrderId == expectedMobile.InFlightOrder.GlobalId)));
            }
        }
    }
}
