using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Moq;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class ActivationRequestedEventCheckerSpec
    {
        private readonly Mobile expectedMobile;
        private readonly ActivationRequestedEventChecker sut;
        private readonly Mock<IGetNewActivationsQuery> getNewActivationsQueryMock;
        private readonly Mock<IMobileCommand> commandMock;

        public ActivationRequestedEventCheckerSpec()
        {
            var inFlightOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });
            expectedMobile = new Mobile(new MobileDataEntity { GlobalId = Guid.NewGuid(), Id = 101, State = "New" }, inFlightOrder, null);
            getNewActivationsQueryMock = new Mock<IGetNewActivationsQuery>();
            commandMock = new Mock<IMobileCommand>();
            getNewActivationsQueryMock.Setup(x => x.Get())
                .Returns(new[] { expectedMobile });

            sut = new ActivationRequestedEventChecker(getNewActivationsQueryMock.Object,
                commandMock.Object);
        }

        [Fact]
        public void ExecuteTheCommand()
        {
            sut.Check();

            this.commandMock.Verify(x => x.Execute(expectedMobile));
        }
    }
}
