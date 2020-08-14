using MinimalEventBus;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Threading;
using SimCards.EventHandlers.Domain;
using SimCards.EventHandlers.BackgroundServices;

namespace SimCards.EventHandlers.Tests.BackgroundServices
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
