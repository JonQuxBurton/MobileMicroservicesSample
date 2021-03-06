﻿using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Handlers;
using SimCards.EventHandlers.Messages;

namespace SimCards.EventHandlers.Domain
{
    public class MessageBusListenerBuilder : IMessageBusListenerBuilder
    {
        private readonly ILogger<ProvisionRequestedHandler> logger;
        private readonly ISimCardOrdersDataStore simCardOrdersDataStore;
        private readonly IExternalSimCardsProviderService simCardWholesaleService;
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;

        public MessageBusListenerBuilder(ILogger<ProvisionRequestedHandler> logger,
            ISimCardOrdersDataStore simCardOrdersDataStore,
            IExternalSimCardsProviderService simCardWholesaleService,
            IMessageBus messageBus,
            ISqsService sqsService,
            IMessagePublisher messagePublisher,
            IMonitoring monitoring)
        {
            this.logger = logger;
            this.simCardOrdersDataStore = simCardOrdersDataStore;
            this.simCardWholesaleService = simCardWholesaleService;
            this.messageBus = messageBus;
            this.sqsService = sqsService;
            this.messagePublisher = messagePublisher;
            this.monitoring = monitoring;
        }

        public IMessageBusListener Build()
        {
            var handler = new ProvisionRequestedHandler(logger, simCardOrdersDataStore, simCardWholesaleService, messagePublisher, monitoring);
            messageBus.Subscribe<ProvisionRequestedMessage, IHandlerAsync<ProvisionRequestedMessage>>(handler);

            return new MessageBusListener(messageBus, sqsService, new MessageDeserializer());
        }
    }
}
