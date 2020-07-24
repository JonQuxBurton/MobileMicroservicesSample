using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;

namespace MobileOrderer.Api.Tests
{
    public static class ServiceProviderHelper
    {
        public static Mock<IServiceProvider> GetMock()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactoryMock.Object);

            return serviceProviderMock;
        }
    }
}
