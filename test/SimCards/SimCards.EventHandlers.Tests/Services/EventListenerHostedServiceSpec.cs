using MinimalEventBus;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Threading;
using SimCards.EventHandlers.Services;
using SimCards.EventHandlers.Domain;

namespace SimCards.EventHandlers.Tests
{
    public class EventListenerHostedServiceSpec
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
