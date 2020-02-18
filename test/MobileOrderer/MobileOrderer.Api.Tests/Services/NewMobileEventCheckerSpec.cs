using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Moq;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class NewMobileEventCheckerSpec
    {
        private readonly Mobile expectedNewMobile;
        private readonly NewMobileEventChecker sut;
        private readonly Mock<IGetNewMobilesQuery> getNewMobilesQueryMock;
        private readonly Mock<IMobileCommand> provisionerCommandMock;

        public NewMobileEventCheckerSpec()
        {
            var inFlightOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });
            expectedNewMobile = new Mobile(new MobileDataEntity { GlobalId = Guid.NewGuid(), Id = 101, State = "New" }, inFlightOrder, null);
            getNewMobilesQueryMock = new Mock<IGetNewMobilesQuery>();
            provisionerCommandMock = new Mock<IMobileCommand>();
            getNewMobilesQueryMock.Setup(x => x.Get())
                .Returns(new[] { expectedNewMobile });

            sut = new NewMobileEventChecker(getNewMobilesQueryMock.Object,
                provisionerCommandMock.Object);
        }

        [Fact]
        public void ExecuteTheCommand()
        {
            sut.Check();

            this.provisionerCommandMock.Verify(x => x.Execute(expectedNewMobile));
        }
    }
}
