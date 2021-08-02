using FluentAssertions;
using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Utils.DateTimes;
using Xunit;

namespace Mobiles.Api.Tests.Domain
{
    namespace MobileSpec
    {
        public class ConstructorShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public ConstructorShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Fact]
            public void CreateMobileInStateNew()
            {
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity{ Id = 101, GlobalId = Guid.NewGuid(), State = "New" });

                sut.State.Should().Be(Mobile.MobileState.New);
            }

            [Fact]
            public void CreateMobile()
            {
                var expectedGlobalId = Guid.NewGuid();
                var expectedId = 101;
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity {Id = expectedId, GlobalId = expectedGlobalId, State = "New" });

                sut.GlobalId.Should().Be(expectedGlobalId);
                sut.Id.Should().Be(expectedId);
            }
        }

        public class ProvisionShould
        {
            private readonly Mobile sut;

            public ProvisionShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    Id = 101,
                    GlobalId = Guid.NewGuid(),
                    State = "New",
                    Orders = new List<OrderDataEntity>()
                    {
                        mobileOrder.GetDataEntity()
                    }
                });

                sut.Provision();
            }

            [Fact]
            public void ChangeCurrentStateToPendingActivation()
            {
                sut.State.Should().Be(Mobile.MobileState.ProcessingProvision);
            }

            [Fact]
            public void SetInProgressOrderStateToNew()
            {
                sut.InProgressOrder.CurrentState.Should().Be(Order.State.New);
            }
        }
        
        public class ProcessingProvisionCompletedShould
        {
            private readonly Mobile sut;

            public ProcessingProvisionCompletedShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    Id = 101,
                    GlobalId = Guid.NewGuid(),
                    State = Mobile.MobileState.ProcessingProvision.ToString(),
                    Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                });

                sut.ProcessingProvisionCompleted();
            }

            [Fact]
            public void ChangeCurrentStateToWaitingForActivate()
            {
                sut.State.Should().Be(Mobile.MobileState.WaitingForActivate);
            }

            [Fact]
            public void SetProvisionOrderStateToCompleted()
            {
                sut.Orders.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInProgressOrderToNull()
            {
                sut.InProgressOrder.Should().BeNull();
            }
        }

        public class ActivateShould
        {
            private readonly Mobile sut;

            public ActivateShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101,
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.WaitingForActivate.ToString()

                    });

                sut.Activate(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));
            }

            [Fact]
            public void ChangeCurrentStateToProcessingActivate()
            {
                sut.State.Should().Be(Mobile.MobileState.ProcessingActivate);
            }

            [Fact]
            public void SetInProgressOrderStateToNew()
            {

                sut.InProgressOrder.CurrentState.Should().Be(Order.State.New);
            }
        }
        
        public class ActivateCompletedShould
        {
            private readonly Mobile sut;

            public ActivateCompletedShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101,
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    });

                sut.ActivateCompleted();

            }

            [Fact]
            public void ChangeCurrentStateToLive()
            {
                sut.State.Should().Be(Mobile.MobileState.Live);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {

                sut.Orders.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInProgressOrderToNull()
            {
                sut.InProgressOrder.Should().BeNull();
            }
        }
        
        public class ActivateRejectedShould
        {
            private readonly Mobile sut;

            public ActivateRejectedShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101,
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    });

                sut.ActivateRejected();
            }

            [Fact]
            public void ChangeCurrentStateToWaitingForActivate()
            {
                sut.State.Should().Be(Mobile.MobileState.WaitingForActivate);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {
                sut.Orders.First().CurrentState.Should().Be(Order.State.Rejected);
            }

            [Fact]
            public void SetInProgressOrderToNull()
            {
                sut.InProgressOrder.Should().BeNull();
            }
        }

        public class CeaseShould
        {
            private readonly Mobile sut;

            public CeaseShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "Live" });

                sut.Cease(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));
            }

            [Fact]
            public void ChangeCurrentStateToProcessingCease()
            {
                sut.State.Should().Be(Mobile.MobileState.ProcessingCease);
            }

            [Fact]
            public void SetInProgressOrderStateToNew()
            {
                sut.InProgressOrder.CurrentState.Should().Be(Order.State.New);
            }
        }

        public class CeaseCompletedShould
        {
            private readonly Mobile sut;

            public CeaseCompletedShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    Id = 101,
                    GlobalId = Guid.NewGuid(),
                    State = Mobile.MobileState.ProcessingCease.ToString(),
                    Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                });

                sut.CeaseCompleted();
            }

            [Fact]
            public void ChangeCurrentStateToCeased()
            {
                sut.State.Should().Be(Mobile.MobileState.Ceased);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {
                sut.Orders.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInProgressOrderToNull()
            {
                sut.InProgressOrder.Should().BeNull();
            }
        }

        public class OrderProcessingShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public OrderProcessingShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Theory]
            [InlineData("PendingActivation")]
            [InlineData("PendingLive")]
            public void SetInProgressOrderStateToProcessing(string mobileState)
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = mobileState ,
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    });

                sut.OrderProcessing();

                sut.InProgressOrder.CurrentState.Should().Be(Order.State.Processing);
            }
        }

        public class OrderSentShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public OrderSentShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Theory]
            [InlineData("PendingActivation")]
            [InlineData("PendingLive")]
            public void SetInProgressOrderStateToSent(string mobileState)
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Processing" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = mobileState,
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    }
                    );

                sut.OrderSent();

                sut.InProgressOrder.CurrentState.Should().Be(Order.State.Sent);
            }
        }
    }
}
