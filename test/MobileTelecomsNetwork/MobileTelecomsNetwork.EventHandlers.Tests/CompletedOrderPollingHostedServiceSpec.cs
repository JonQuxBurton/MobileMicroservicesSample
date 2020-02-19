using Microsoft.Extensions.Logging;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Services;
using Moq;
using System;
using Xunit;

namespace MobileTelecomsNetwork.EventHandlers.Tests
{
    public class CompletedOrderPollingHostedServiceSpec
    {
        public class DoWorkShould
        {
            [Fact]
            public void CallExternalService()
            {
                var config = new Config
                {
                    ExternalMobileTelecomsNetworkApiUrl = "http://api:5000"
                };
                var expectedOrder = new ActivationOrder()
                { 
                    MobileOrderId = Guid.NewGuid()
                };
                var expectedUrl = new Uri($"{config.ExternalMobileTelecomsNetworkApiUrl}/api/orders/{expectedOrder.MobileOrderId}");
                var externalOrder = new ExternalOrder {
                    Name = "Neil Armstrong",
                    Status = "Completed"
                };
                var dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.GetSent())
                    .Returns(new[] { expectedOrder });
                var activationOrderChecker = new Mock<IActivationOrderChecker>();

                var sut = new CompletedOrderPollingHostedService(Mock.Of<ILogger<CompletedOrderPollingHostedService>>(), 
                    dataStoreMock.Object,
                    activationOrderChecker.Object);

                sut.DoWork(new object());

                activationOrderChecker.Verify(x => x.Check(expectedOrder));
            }
        }
    }
}
