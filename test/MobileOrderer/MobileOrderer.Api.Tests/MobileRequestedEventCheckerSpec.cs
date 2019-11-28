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

namespace MobileOrderer.Api.Tests
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
            var inFlightOrder = new MobileOrder(Guid.NewGuid(), "Neil", "0123456789", "New");
            expectedNewMobile = new Mobile(Mobile.State.New, Guid.NewGuid(), 101, inFlightOrder, null);
            messagePublisher = new Mock<IMessagePublisher>();
            mobileRepositoryMock = new Mock<IRepository<Mobile>>();
            getNewMobilesQueryMock = new Mock<IGetNewMobilesQuery>();
            this.getNewMobilesQueryMock.Setup(x => x.GetNew())
                .Returns(new[] { expectedNewMobile });

            sut = new MobileRequestedEventChecker(messagePublisher.Object, getNewMobilesQueryMock.Object, mobileRepositoryMock.Object);
        }

        [Fact]
        public void SavesTheMobile()
        {
            sut.Check();

            this.mobileRepositoryMock.Verify(x => x.Save(expectedNewMobile));
        }        
        
        [Fact]
        public void ProvisionsTheMobile()
        {
            sut.Check();

            expectedNewMobile.CurrentState.Should().Be(Mobile.State.PendingLive);
        }

        [Fact]
        public void PublishesMobileRequested()
        {
            sut.Check();

            messagePublisher.Verify(x => x.PublishAsync(It.Is<MobileRequestedMessage>(
                y => y.MobileOrderId == expectedNewMobile.GlobalId &&
                        y.Name == expectedNewMobile.InFlightOrder.Name &&
                        y.ContactPhoneNumber == expectedNewMobile.InFlightOrder.ContactPhoneNumber)));
        }
    }
}
