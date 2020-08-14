using FluentAssertions;
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

                sut = new MobileProvisionRequestedEventChecker(getNewMobilesQueryMock.Object,
                    repositoryMock.Object);
            }

            [Fact]
            public void SetTheMobileToProcessingProvisioning()
            {
                sut.Check();

                expectedNewMobile.CurrentState.Should().Be(Api.Domain.Mobile.State.ProcessingProvisioning);
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