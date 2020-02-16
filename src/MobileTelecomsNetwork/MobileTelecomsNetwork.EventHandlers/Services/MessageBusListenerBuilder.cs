using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Handlers;
using MobileTelecomsNetwork.EventHandlers.Messages;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public class MessageBusListenerBuilder : IMessageBusListenerBuilder
    {
        private readonly ILogger<ActivationRequestedHandler> logger;
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IDataStore dataStore;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;

        public MessageBusListenerBuilder(ILogger<ActivationRequestedHandler> logger,
            IMessageBus messageBus,
            ISqsService sqsService,
            IMessagePublisher messagePublisher,
            IDataStore dataStore,
            IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService)
        {
            this.logger = logger;
            this.messageBus = messageBus;
            this.sqsService = sqsService;
            this.messagePublisher = messagePublisher;
            this.dataStore = dataStore;
            this.externalMobileTelecomsNetworkService = externalMobileTelecomsNetworkService;
        }

        public IMessageBusListener Build()
        {
            var handler = new ActivationRequestedHandler(logger, this.dataStore, this.externalMobileTelecomsNetworkService, this.messagePublisher);
            messageBus.Subscribe<ActivationRequestedMessage, IHandlerAsync<ActivationRequestedMessage>>(handler);

            return new MessageBusListener(messageBus, sqsService, new MessageDeserializer());
        }
    }
}
