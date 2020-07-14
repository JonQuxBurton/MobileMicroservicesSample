using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Moq;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class CeaseRequestedEventCheckerSpec
    {
        private readonly Mobile expectedMobile;
        private readonly CeaseRequestedEventChecker sut;
        private readonly Mock<IGetNewCeasesQuery> getMobilesQueryMock;
        private readonly Mock<IMobileCommand> commandMock;

        public CeaseRequestedEventCheckerSpec()
        {
            var inFlightOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });
            expectedMobile = new Mobile(new MobileDataEntity { GlobalId = Guid.NewGuid(), Id = 101, State = "Live" }, inFlightOrder, null);
            getMobilesQueryMock = new Mock<IGetNewCeasesQuery>();
            commandMock = new Mock<IMobileCommand>();
            getMobilesQueryMock.Setup(x => x.Get())
                .Returns(new[] { expectedMobile });

            sut = new CeaseRequestedEventChecker(getMobilesQueryMock.Object,
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
