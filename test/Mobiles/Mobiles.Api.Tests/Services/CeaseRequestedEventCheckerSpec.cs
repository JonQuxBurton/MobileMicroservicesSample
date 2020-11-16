using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Messages;
using Mobiles.Api.Services;
using Moq;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Xunit;

namespace Mobiles.Api.Tests.Services
{
    namespace CeaseRequestedEventCheckerSpec
    {
        public class CheckShould
        {
            private readonly Mobile expectedMobile;
            private readonly CeaseRequestedEventChecker sut;
            private readonly Mock<IGetNewCeasesQuery> getMobilesQueryMock;
            private readonly Mock<IRepository<Mobile>> repositoryMock;
            private readonly Mock<IMessagePublisher> messagePublisherMock;

            public CheckShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity()
                {
                    State = Mobile.MobileState.ProcessingCease.ToString(),
                    Orders = new List<OrderDataEntity>()
                    {
                        new OrderDataEntity
                        {
                            State = Order.State.New.ToString()
                        }
                    }
                });
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

                expectedMobile.State.Should().Be(Mobile.MobileState.ProcessingCease);
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
