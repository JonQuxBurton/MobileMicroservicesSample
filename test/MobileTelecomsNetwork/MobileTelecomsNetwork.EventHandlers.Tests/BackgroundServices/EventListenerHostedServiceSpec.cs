using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MobileTelecomsNetwork.EventHandlers.BackgroundServices;
using MobileTelecomsNetwork.EventHandlers.Domain;
using Moq;
using System.Threading;
using Xunit;

namespace MobileTelecomsNetwork.EventHandlers.Tests.BackgroundServices
{
    namespace EventListenerHostedServiceSpec
    {
        public class RunShould
        {
            [Fact]
            public void StartListening()
            {
                var messageBusMock = new Mock<IMessageBusListener>();
                var messageBusListenerBuilderMock = new Mock<IMessageBusListenerBuilder>();
                messageBusListenerBuilderMock.Setup(x => x.Build()).Returns(messageBusMock.Object);
                var sut = new EventListenerHostedService(Mock.Of<ILogger<EventListenerHostedService>>(), messageBusListenerBuilderMock.Object);

                sut.StartAsync(new CancellationToken());

                messageBusMock.Verify(x => x.StartListening());
            }
        }
    }
}
