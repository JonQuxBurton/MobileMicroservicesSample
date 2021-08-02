using FluentAssertions;
using Mobiles.Api.Domain;
using System;
using Xunit;

namespace Mobiles.Api.Tests.Domain
{
    namespace OrderSpec
    {
        public class ProcessShould
        {
            [Fact]
            public void ChangeStateToProcessing()
            {
                var sut = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });

                sut.Process();

                sut.CurrentState.Should().Be(Order.State.Processing);
            }
        }

        public class SendShould
        {
            [Fact]
            public void ChangeStateToSent()
            {
                var sut = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "Processing" });

                sut.Send();

                sut.CurrentState.Should().Be(Order.State.Sent);
            }
        }
        
        public class CompleteShould
        {
            [Fact]
            public void ChangeStateToCompleted()
            {
                var sut = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "Sent" });

                sut.Complete();

                sut.CurrentState.Should().Be(Order.State.Completed);
            }
        }

        public class RejectShould
        {
            [Fact]
            public void ChangeStateToRejected()
            {
                var sut = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "Sent" });

                sut.Reject();

                sut.CurrentState.Should().Be(Order.State.Rejected);
            }
        }
    }
}
