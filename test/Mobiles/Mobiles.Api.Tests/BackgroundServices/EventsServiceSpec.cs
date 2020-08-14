using Microsoft.Extensions.Logging;
using MinimalEventBus;
using Mobiles.Api.Services;
using Moq;
using System.Threading;
using Xunit;

namespace Mobiles.Api.Tests.BackgroundServices
{
    public class EventsServiceSpec
    {
        private readonly Mock<IMessageBusListenerBuilder> messageBusListenerBuilderMock;
        private readonly Mock<IMessageBusListener> messageBusListenerMock;
        private readonly Mock<ILogger<EventsService>> loggerMock;
        private readonly EventsService sut;

        public EventsServiceSpec()
        {
            messageBusListenerBuilderMock = new Mock<IMessageBusListenerBuilder>();
            messageBusListenerMock = new Mock<IMessageBusListener>();
            loggerMock = new Mock<ILogger<EventsService>>();
            messageBusListenerBuilderMock.Setup(x => x.Build())
                .Returns(messageBusListenerMock.Object);
            sut = new EventsService(loggerMock.Object, messageBusListenerBuilderMock.Object);
        }

        [Fact]
        public void BuildsMessageBusListener()
        {
            sut.StartAsync(new CancellationToken());

            messageBusListenerBuilderMock.Verify(x => x.Build(), Times.Once);
        }

        [Fact]
        public void StartsListener()
        {
            sut.StartAsync(new CancellationToken());

            messageBusListenerMock.Verify(x => x.StartListening(), Times.Once);
        }
    }
}
