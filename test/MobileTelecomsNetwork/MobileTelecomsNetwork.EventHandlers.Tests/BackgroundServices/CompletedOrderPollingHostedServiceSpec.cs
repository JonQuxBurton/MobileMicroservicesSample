using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MobileTelecomsNetwork.EventHandlers.BackgroundServices;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using Moq;
using System;
using Xunit;

namespace MobileTelecomsNetwork.EventHandlers.Tests.BackgroundServices
{
    namespace CompletedOrderPollingHostedServiceSpec
    {
        public class DoWorkShould
        {
            [Fact]
            public async void CallExternalService()
            {
                var config = new Config
                {
                    ExternalMobileTelecomsNetworkApiUrl = "http://api:5000"
                };
                var expectedOrder = new Order()
                {
                    MobileOrderId = Guid.NewGuid()
                };
                var dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.GetSent())
                    .Returns(new[] { expectedOrder });
                var activateOrderChecker = new Mock<IOrderCompletedChecker>();
                var options = Options.Create(config);

                var sut = new CompletedOrderPollingHostedService(options, Mock.Of<ILogger<CompletedOrderPollingHostedService>>(),
                    dataStoreMock.Object,
                    activateOrderChecker.Object);

                await sut.DoWork();

                activateOrderChecker.Verify(x => x.Check(expectedOrder));
            }
        }
    }
}
