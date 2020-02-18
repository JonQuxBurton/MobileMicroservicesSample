using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Moq;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class ProcessingProvisioningEventCheckerSpec
    {
        private readonly Mobile expectedMobile;
        private readonly ProcessingProvisioningEventChecker sut;
        private readonly Mock<IGetProcessingProvisioningMobilesQuery> queryMock;
        private readonly Mock<IMobileCommand> commandMock;

        public ProcessingProvisioningEventCheckerSpec()
        {
            var inFlightOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });
            expectedMobile = new Mobile(new MobileDataEntity { GlobalId = Guid.NewGuid(), Id = 101, State = "New" }, inFlightOrder, null);
            queryMock = new Mock<IGetProcessingProvisioningMobilesQuery>();
            commandMock = new Mock<IMobileCommand>();
            queryMock.Setup(x => x.Get())
                .Returns(new[] { expectedMobile });

            sut = new ProcessingProvisioningEventChecker(queryMock.Object,
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
