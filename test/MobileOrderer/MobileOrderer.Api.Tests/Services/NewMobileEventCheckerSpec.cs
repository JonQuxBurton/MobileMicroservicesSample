using FluentAssertions;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Moq;
using Utils.DomainDrivenDesign;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public static class NewMobileEventCheckerSpec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public class CheckShould
        {
            private readonly Mobile expectedNewMobile;
            private readonly NewMobileEventChecker sut;
            private readonly Mock<IGetNewMobilesQuery> getNewMobilesQueryMock;
            private readonly Mock<IRepository<Mobile>> repositoryMock;

            public CheckShould()
            {
                expectedNewMobile = new Mobile(new MobileDataEntity()
                {
                    State = "New"
                }, null);
                getNewMobilesQueryMock = new Mock<IGetNewMobilesQuery>();
                getNewMobilesQueryMock.Setup(x => x.Get())
                    .Returns(new[] { expectedNewMobile });
                repositoryMock = new Mock<IRepository<Mobile>>();

                sut = new NewMobileEventChecker(getNewMobilesQueryMock.Object,
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