using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Moq;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class CancelRequestedEventCheckerSpec
    {
        private readonly Mobile expectedMobile;
        private readonly CancelRequestedEventChecker sut;
        private readonly Mock<IGetNewCancelsQuery> getMobilesQueryMock;
        private readonly Mock<IMobileCommand> commandMock;

        public CancelRequestedEventCheckerSpec()
        {
            var inFlightOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });
            expectedMobile = new Mobile(new MobileDataEntity { GlobalId = Guid.NewGuid(), Id = 101, State = "Live" }, inFlightOrder, null);
            getMobilesQueryMock = new Mock<IGetNewCancelsQuery>();
            commandMock = new Mock<IMobileCommand>();
            getMobilesQueryMock.Setup(x => x.Get())
                .Returns(new[] { expectedMobile });

            sut = new CancelRequestedEventChecker(getMobilesQueryMock.Object,
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
