using MinimalEventBus;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Threading;
using SimCards.EventHandlers.Services;

namespace SimCards.EventHandlers.Tests
{
    public class HostedServiceSpec
    {
        public class RunShould
        {
            [Fact]
            public void StartListening()
            {
                var messageBusMock = new Mock<IMessageBusListener>();
                var messageBusListenerBuilderMock = new Mock<IMessageBusListenerBuilder>();
                messageBusListenerBuilderMock.Setup(x => x.Build()).Returns(messageBusMock.Object);
                var sut = new HostedService(Mock.Of<ILogger<HostedService>>(), messageBusListenerBuilderMock.Object);

                sut.StartAsync(new CancellationToken());

                messageBusMock.Verify(x => x.StartListening());
            }
        }
    }
}
