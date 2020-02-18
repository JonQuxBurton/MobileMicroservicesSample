using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MobileOrderer.Api.Services;
using Moq;
using System.Threading;
using Xunit;

namespace MobileOrderer.Api.Tests.Services
{
    public class EventsServiceSpec
    {
        Mock<IMessageBusListenerBuilder> messageBusListenerBuilderMock;
        Mock<IMessageBusListener> messageBusListenerMock;
        Mock<ILogger<EventsService>> loggerMock;
        EventsService sut;

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
