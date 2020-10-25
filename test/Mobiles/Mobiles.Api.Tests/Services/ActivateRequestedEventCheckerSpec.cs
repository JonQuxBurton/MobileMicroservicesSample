using System.Linq;
using System.Threading.Tasks;
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
    namespace ActivateRequestedEventCheckerSpec
    {
        public class CheckShould
        {
            private readonly Mobile expectedMobile;
            private readonly ActivateRequestedEventChecker sut;
            private readonly Mock<IGetNewActivatesQuery> getNewActivationsQueryMock;
            private readonly Mock<IRepository<Mobile>> repositoryMock;
            private readonly Mock<IMessagePublisher> messagePublisherMock;

            public CheckShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = Mobile.State.ProcessingActivate.ToString()
                }, new Order(new OrderDataEntity
                {
                    State = Order.State.New.ToString(),
                    ActivationCode = "BAC132"
                }));

                getNewActivationsQueryMock = new Mock<IGetNewActivatesQuery>();
                getNewActivationsQueryMock.Setup(x => x.Get())
                    .Returns(new[] { expectedMobile });
                repositoryMock = new Mock<IRepository<Mobile>>();
                messagePublisherMock = new Mock<IMessagePublisher>();
                messagePublisherMock.Setup(x => x.PublishAsync(It.IsAny<ActivateRequestedMessage>()))
                    .Returns(Task.FromResult(true));
                var loggerMock = new Mock<ILogger<ActivateRequestedEventChecker>>();

                sut = new ActivateRequestedEventChecker(loggerMock.Object, getNewActivationsQueryMock.Object, repositoryMock.Object, messagePublisherMock.Object);
            }

            [Fact]
            public void SetTheMobileToProcessingActivate()
            {
                sut.Check();

                expectedMobile.CurrentState.Should().Be(Mobile.State.ProcessingActivate);
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
            public void PublishActivationRequestedMessage()
            {
                sut.Check();

                messagePublisherMock.Verify(x => x.PublishAsync(It.Is<ActivateRequestedMessage>(
                    y => y.MobileOrderId == expectedMobile.InFlightOrder.GlobalId &&
                            y.ActivationCode == expectedMobile.InFlightOrder.ActivationCode)));
            }
        }
    }
}
