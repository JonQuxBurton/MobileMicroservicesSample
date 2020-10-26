using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Services;
using Moq;
using Utils.DomainDrivenDesign;
using Xunit;

namespace Mobiles.Api.Tests.Services
{
    namespace ProvisionRequestedEventCheckerSpec
    {
        public class CheckShould
        {
            private readonly Mobile expectedNewMobile;
            private readonly MobileProvisionRequestedEventChecker sut;
            private readonly Mock<IGetNeProvisionsQuery> getNewMobilesQueryMock;
            private readonly Mock<IRepository<Mobile>> repositoryMock;

            public CheckShould()
            {
                expectedNewMobile = new Mobile(new MobileDataEntity()
                {
                    State = "New"
                }, null);
                getNewMobilesQueryMock = new Mock<IGetNeProvisionsQuery>();
                getNewMobilesQueryMock.Setup(x => x.Get())
                    .Returns(new[] { expectedNewMobile });
                repositoryMock = new Mock<IRepository<Mobile>>();
                var loggerMock = new Mock<ILogger<MobileProvisionRequestedEventChecker>>();

                sut = new MobileProvisionRequestedEventChecker(loggerMock.Object, getNewMobilesQueryMock.Object,
                    repositoryMock.Object);
            }

            [Fact]
            public void SetTheMobileToProcessingProvision()
            {
                sut.Check();

                expectedNewMobile.CurrentState.Should().Be(Api.Domain.Mobile.State.ProcessingProvision);
            }

            [Fact]
            public void UpdateTheRepository()
            {
                sut.Check();

                repositoryMock.Verify(x => x.Update(expectedNewMobile));
            }
        }
    }
}