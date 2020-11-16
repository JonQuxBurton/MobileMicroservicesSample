using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Handlers;
using Mobiles.Api.Messages;
using Moq;
using System;
using System.Collections.Generic;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Xunit;

namespace Mobiles.Api.Tests.Handlers
{
    namespace ProvisionOrderSentHandlerSpec
    {
        public class HandleShould
        {
            private readonly MobileProvisionOrderSentHandler sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetMobileByOrderIdQuery> getMobileByOrderIdQueryMock;

            private readonly Mobile expectedMobile;
            private readonly ProvisionOrderSentMessage inputMessage;

            public HandleShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                var inFlightOrder = new Order(new OrderDataEntity()
                {
                    State = "Processing"
                });
                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    State = Mobile.MobileState.ProcessingProvision.ToString(),
                    Orders = new List<OrderDataEntity>() { inFlightOrder.GetDataEntity() }
                });
                inputMessage = new ProvisionOrderSentMessage()
                {
                    MobileOrderId = expectedMobile.GlobalId
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                getMobileByOrderIdQueryMock = new Mock<IGetMobileByOrderIdQuery>();
                var loggerMock = new Mock<ILogger<MobileProvisionOrderSentHandler>>();

                getMobileByOrderIdQueryMock.Setup(x => x.Get(inputMessage.MobileOrderId))
                    .Returns(expectedMobile);

                var serviceProviderMock = ServiceProviderHelper.GetMock();
                serviceProviderMock.Setup(x => x.GetService(typeof(IGetMobileByOrderIdQuery))).Returns(getMobileByOrderIdQueryMock.Object);
                serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<Mobile>))).Returns(mobileRepositoryMock.Object);

                sut = new MobileProvisionOrderSentHandler(loggerMock.Object, serviceProviderMock.Object);
            }

            [Fact]
            public async void SetTheMobilesInFlightOrderToSent()
            {
                await sut.Handle(inputMessage);

                expectedMobile.State.Should().Be(Mobile.MobileState.ProcessingProvision);
                expectedMobile.InProgressOrder.CurrentState.Should().Be(Order.State.Sent);
            }

            [Fact]
            public async void UpdateTheMobileInTheRepository()
            {
                await sut.Handle(inputMessage);

                this.mobileRepositoryMock.Verify(x => x.Update(expectedMobile));
            }

            [Fact]
            public async void ReturnTrueWhenSuccessful()
            {
                var actual = await sut.Handle(inputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void ReturnFalseWhenNotSuccessful()
            {
                var nonExistantMobileMessage = new ProvisionOrderSentMessage()
                {
                    MobileOrderId = Guid.Empty
                };

                var actual = await sut.Handle(nonExistantMobileMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
