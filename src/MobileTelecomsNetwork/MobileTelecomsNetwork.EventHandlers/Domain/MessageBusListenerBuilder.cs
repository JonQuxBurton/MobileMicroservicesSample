using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Handlers;
using MobileTelecomsNetwork.EventHandlers.Messages;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class MessageBusListenerBuilder : IMessageBusListenerBuilder
    {
        private readonly ILogger<ActivateRequestedHandler> logger;
        private readonly ILogger<CeaseRequestedHandler> ceaseRequestedHandlerLogger;
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IDataStore dataStore;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;
        private readonly IMonitoring monitoring;

        public MessageBusListenerBuilder(ILogger<ActivateRequestedHandler> logger,
            ILogger<CeaseRequestedHandler> ceaseRequestedHandlerLogger,
            IMessageBus messageBus,
            ISqsService sqsService,
            IMessagePublisher messagePublisher,
            IDataStore dataStore,
            IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService,
            IMonitoring monitoring)
        {
            this.logger = logger;
            this.ceaseRequestedHandlerLogger = ceaseRequestedHandlerLogger;
            this.messageBus = messageBus;
            this.sqsService = sqsService;
            this.messagePublisher = messagePublisher;
            this.dataStore = dataStore;
            this.externalMobileTelecomsNetworkService = externalMobileTelecomsNetworkService;
            this.monitoring = monitoring;
        }

        public IMessageBusListener Build()
        {
            var handler = new ActivateRequestedHandler(logger, dataStore, externalMobileTelecomsNetworkService, messagePublisher, monitoring);
            messageBus.Subscribe<ActivateRequestedMessage, IHandlerAsync<ActivateRequestedMessage>>(handler);

            var ceaseRequestedHandler = new CeaseRequestedHandler(ceaseRequestedHandlerLogger, dataStore, externalMobileTelecomsNetworkService, messagePublisher, monitoring);
            messageBus.Subscribe<CeaseRequestedMessage, IHandlerAsync<CeaseRequestedMessage>>(ceaseRequestedHandler);

            return new MessageBusListener(messageBus, sqsService, new MessageDeserializer());
        }
    }
}
