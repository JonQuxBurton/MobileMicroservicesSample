using FluentAssertions;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Moq;
using Utils.DomainDrivenDesign;
using Xunit;
using static MobileOrderer.Api.Domain.Mobile;

namespace MobileOrderer.Api.Tests.Services
{
    public class ProvisionCommandSpec
    {
        public class ExecuteShould
        {
            private readonly Mobile expectedMobile;
            private readonly Mock<IRepository<Mobile>> repositoryMock;
            private readonly ProvisionCommand sut;

            public ExecuteShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = "New"
                }, null);
                repositoryMock = new Mock<IRepository<Mobile>>();
                sut = new ProvisionCommand(repositoryMock.Object);
            }

            [Fact]
            public void ProvisionTheMobile()
            {
                sut.Execute(expectedMobile);

                expectedMobile.CurrentState.Should().Be(State.ProcessingProvisioning);
            }

            [Fact]
            public void UpdateTheRepository()
            {
                sut.Execute(expectedMobile);

                repositoryMock.Verify(x => x.Update(expectedMobile));
            }
        }
    }
}
