using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Services;
using Moq;
using Utils.DateTimes;
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
            private readonly Mock<IGetNewProvisionsQuery> getNewMobilesQueryMock;
            private readonly Mock<IRepository<Mobile>> repositoryMock;

            public CheckShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                expectedNewMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity()
                {
                    State = "New"
                });
                getNewMobilesQueryMock = new Mock<IGetNewProvisionsQuery>();
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

                expectedNewMobile.State.Should().Be(Api.Domain.Mobile.MobileState.ProcessingProvision);
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