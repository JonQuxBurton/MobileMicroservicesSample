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
        private readonly ILogger<ProvisionOrderSentHandler> orderSentLogger;
        private readonly ILogger<ProvisionOrderCompletedHandler> provisioningOrderCompletedLogger;
        private readonly ILogger<ActivationOrderSentHandler> activationOrderSentLogger;
        private readonly ILogger<CeaseOrderSentHandler> ceaseOrderSentLogger;
        private readonly ILogger<ActivationOrderCompletedHandler> activationOrderCompletedLogger;
        private readonly ILogger<CeaseOrderCompletedHandler> ceaseOrderCompletedLogger;
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IMonitoring monitoring;
        private readonly IServiceProvider serviceProvider;

        public MessageBusListenerBuilder(ILogger<ProvisionOrderSentHandler> orderSentLogger, 
            ILogger<ProvisionOrderCompletedHandler> provisioningOrderCompletedLogger,
            ILogger<ActivationOrderSentHandler> activationOrderSentLogger,
            ILogger<CeaseOrderSentHandler> ceaseOrderSentLogger,
            ILogger<ActivationOrderCompletedHandler> activationOrderCompletedLogger,
            ILogger<CeaseOrderCompletedHandler> ceaseOrderCompletedLogger,
            IMessageBus messageBus, 
            ISqsService sqsService,
            IMonitoring monitoring,
            IServiceProvider serviceProvider)
        {
            this.orderSentLogger = orderSentLogger;
            this.provisioningOrderCompletedLogger = provisioningOrderCompletedLogger;
            this.activationOrderSentLogger = activationOrderSentLogger;
            this.ceaseOrderSentLogger = ceaseOrderSentLogger;
            this.activationOrderCompletedLogger = activationOrderCompletedLogger;
            this.ceaseOrderCompletedLogger = ceaseOrderCompletedLogger;
            this.messageBus = messageBus;
            this.sqsService = sqsService;
            this.monitoring = monitoring;
            this.serviceProvider = serviceProvider;
        }

        public IMessageBusListener Build()
        {
            var provisionOrderHandler = new ProvisionOrderSentHandler(orderSentLogger, serviceProvider);
            messageBus.Subscribe<OrderSentMessage, IHandlerAsync<OrderSentMessage>>(provisionOrderHandler);

            var provisionOrderCompletedHandler = new ProvisionOrderCompletedHandler(provisioningOrderCompletedLogger, monitoring, serviceProvider);
            messageBus.Subscribe<ProvisioningOrderCompletedMessage, IHandlerAsync<ProvisioningOrderCompletedMessage>>(provisionOrderCompletedHandler);

            var activationOrderSentHandler = new ActivationOrderSentHandler(activationOrderSentLogger, serviceProvider);
            messageBus.Subscribe<ActivationOrderSentMessage, IHandlerAsync<ActivationOrderSentMessage>>(activationOrderSentHandler);

            var activationOrderCompletedHandler = new ActivationOrderCompletedHandler(activationOrderCompletedLogger, monitoring, serviceProvider);
            messageBus.Subscribe<ActivationOrderCompletedMessage, IHandlerAsync<ActivationOrderCompletedMessage>>(activationOrderCompletedHandler);

            var ceaseOrderSentHandler = new CeaseOrderSentHandler(ceaseOrderSentLogger, serviceProvider);
            messageBus.Subscribe<CeaseOrderSentMessage, IHandlerAsync<CeaseOrderSentMessage>>(ceaseOrderSentHandler);

            var ceaseOrderCompletedHandler = new CeaseOrderCompletedHandler(ceaseOrderCompletedLogger, monitoring, serviceProvider);
            messageBus.Subscribe<CeaseOrderCompletedMessage, IHandlerAsync<CeaseOrderCompletedMessage>>(ceaseOrderCompletedHandler);

            return new MessageBusListener(messageBus, sqsService, new MessageDeserializer());
        }
    }
}
