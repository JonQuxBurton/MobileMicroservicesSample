using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Handlers;
using MobileOrderer.Api.Messages;
using System;

namespace MobileOrderer.Api.Services
{
    public class MessageBusListenerBuilder : IMessageBusListenerBuilder
    {
        private readonly ILogger<MobileProvisionOrderSentHandler> orderSentLogger;
        private readonly ILogger<OrderCompletedHandler> provisioningOrderCompletedLogger;
        private readonly ILogger<ActivateOrderSentHandler> activateOrderSentLogger;
        private readonly ILogger<CeaseOrderSentHandler> ceaseOrderSentLogger;
        private readonly ILogger<ActivateOrderCompletedHandler> activateOrderCompletedLogger;
        private readonly ILogger<CeaseOrderCompletedHandler> ceaseOrderCompletedLogger;
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public MessageBusListenerBuilder(ILogger<MobileProvisionOrderSentHandler> orderSentLogger, 
            ILogger<OrderCompletedHandler> provisioningOrderCompletedLogger,
            ILogger<ActivateOrderSentHandler> activateOrderSentLogger,
            ILogger<CeaseOrderSentHandler> ceaseOrderSentLogger,
            ILogger<ActivateOrderCompletedHandler> activateOrderCompletedLogger,
            ILogger<CeaseOrderCompletedHandler> ceaseOrderCompletedLogger,
            IMessageBus messageBus, 
            ISqsService sqsService,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.orderSentLogger = orderSentLogger;
            this.provisioningOrderCompletedLogger = provisioningOrderCompletedLogger;
            this.activateOrderSentLogger = activateOrderSentLogger;
            this.ceaseOrderSentLogger = ceaseOrderSentLogger;
            this.activateOrderCompletedLogger = activateOrderCompletedLogger;
            this.ceaseOrderCompletedLogger = ceaseOrderCompletedLogger;
            this.messageBus = messageBus;
            this.sqsService = sqsService;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public IMessageBusListener Build()
        {
            var provisionOrderHandler = new MobileProvisionOrderSentHandler(orderSentLogger, serviceProvider);
            messageBus.Subscribe<ProvisionOrderSentMessage, IHandlerAsync<ProvisionOrderSentMessage>>(provisionOrderHandler);

            var provisionOrderCompletedHandler = new OrderCompletedHandler(provisioningOrderCompletedLogger, monitoring, serviceProvider);
            messageBus.Subscribe<ProvisionOrderCompletedMessage, IHandlerAsync<ProvisionOrderCompletedMessage>>(provisionOrderCompletedHandler);

            var activateOrderSentHandler = new ActivateOrderSentHandler(activateOrderSentLogger, serviceProvider);
            messageBus.Subscribe<ActivateOrderSentMessage, IHandlerAsync<ActivateOrderSentMessage>>(activateOrderSentHandler);

            var activateOrderCompletedHandler = new ActivateOrderCompletedHandler(activateOrderCompletedLogger, monitoring, serviceProvider);
            messageBus.Subscribe<ActivateOrderCompletedMessage, IHandlerAsync<ActivateOrderCompletedMessage>>(activateOrderCompletedHandler);

            var ceaseOrderSentHandler = new CeaseOrderSentHandler(ceaseOrderSentLogger, serviceProvider);
            messageBus.Subscribe<CeaseOrderSentMessage, IHandlerAsync<CeaseOrderSentMessage>>(ceaseOrderSentHandler);

            var ceaseOrderCompletedHandler = new CeaseOrderCompletedHandler(ceaseOrderCompletedLogger, monitoring, serviceProvider);
            messageBus.Subscribe<CeaseOrderCompletedMessage, IHandlerAsync<CeaseOrderCompletedMessage>>(ceaseOrderCompletedHandler);

            return new MessageBusListener(messageBus, sqsService, new MessageDeserializer());
        }
    }
}
