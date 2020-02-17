using FluentAssertions;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using MobileOrderer.Api.Services;
using Moq;
using System;
using Utils.DomainDrivenDesign;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class MobileRequestedEventCheckerSpec
    {
        private readonly Mobile expectedNewMobile;
        private readonly MobileRequestedEventChecker sut;
        private readonly Mock<IGetNewMobilesQuery> getNewMobilesQueryMock;
        private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
        private readonly Mock<IMessagePublisher> messagePublisher;

        public MobileRequestedEventCheckerSpec()
        {
            var inFlightOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });
            expectedNewMobile = new Mobile(new MobileDataEntity { GlobalId = Guid.NewGuid(), Id = 101, State = "New" }, inFlightOrder, null);
            messagePublisher = new Mock<IMessagePublisher>();
            mobileRepositoryMock = new Mock<IRepository<Mobile>>();
            getNewMobilesQueryMock = new Mock<IGetNewMobilesQuery>();
            getNewMobilesQueryMock.Setup(x => x.GetNew())
                .Returns(new[] { expectedNewMobile });

            sut = new MobileRequestedEventChecker(messagePublisher.Object, getNewMobilesQueryMock.Object, mobileRepositoryMock.Object);
        }

        [Fact]
        public void UpdateTheMobile()
        {
            sut.Check();

            mobileRepositoryMock.Verify(x => x.Update(expectedNewMobile));
        }

        [Fact]
        public void ProvisionsTheMobile()
        {
            sut.Check();

            expectedNewMobile.CurrentState.Should().Be(Mobile.State.ProcessingProvisioning);
        }

        [Fact]
        public void PublishesMobileRequested()
        {
            sut.Check();

            messagePublisher.Verify(x => x.PublishAsync(It.Is<MobileRequestedMessage>(
                y => y.MobileOrderId == expectedNewMobile.InFlightOrder.GlobalId &&
                        y.Name == expectedNewMobile.InFlightOrder.Name &&
                        y.ContactPhoneNumber == expectedNewMobile.InFlightOrder.ContactPhoneNumber)));
        }
    }
}
