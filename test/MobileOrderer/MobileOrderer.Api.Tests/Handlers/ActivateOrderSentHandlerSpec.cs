﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Handlers;
using MobileOrderer.Api.Messages;
using Moq;
using System;
using Utils.DomainDrivenDesign;
using Xunit;

namespace MobileOrderer.Api.Tests.Handlers
{
    public static class ActivateOrderSentHandlerSpec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public class HandleShould
        {
            private readonly ActivateOrderSentHandler sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetMobileByOrderIdQuery> getMobileByOrderIdQueryMock;

            private readonly Mobile expectedMobile;
            private readonly ActivateOrderSentMessage inputMessage;

            public HandleShould()
            {
                var inFlightOrder = new Order(new OrderDataEntity()
                {
                    State = "Processing"
                });
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    State = "ProcessingActivation"
                }, inFlightOrder);
                inputMessage = new ActivateOrderSentMessage()
                {
                    MobileOrderId = expectedMobile.GlobalId
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                getMobileByOrderIdQueryMock = new Mock<IGetMobileByOrderIdQuery>();
                var loggerMock = new Mock<ILogger<ActivateOrderSentHandler>>();

                getMobileByOrderIdQueryMock.Setup(x => x.Get(inputMessage.MobileOrderId))
                    .Returns(expectedMobile);

                var serviceProviderMock = ServiceProviderHelper.GetMock();
                serviceProviderMock.Setup(x => x.GetService(typeof(IGetMobileByOrderIdQuery))).Returns(getMobileByOrderIdQueryMock.Object);
                serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<Mobile>))).Returns(mobileRepositoryMock.Object);

                sut = new ActivateOrderSentHandler(loggerMock.Object, serviceProviderMock.Object);
            }

            [Fact]
            public async void SetTheMobilesInFlightOrderToSent()
            {
                await sut.Handle(inputMessage);

                expectedMobile.CurrentState.Should().Be(Mobile.State.ProcessingActivation);
                expectedMobile.InFlightOrder.CurrentState.Should().Be(Order.State.Sent);
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
                var nonExistantMobileMessage = new ActivateOrderSentMessage()
                {
                    MobileOrderId = Guid.Empty
                };

                var actual = await sut.Handle(nonExistantMobileMessage);

                actual.Should().BeFalse();
            }
        }
    }
}