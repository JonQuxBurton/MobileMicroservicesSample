using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Handlers;
using MobileOrderer.Api.Messages;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Services
{
    public class MessageBusListenerBuilder : IMessageBusListenerBuilder
    {
        private readonly ILogger<OrderSentHandler> orderSentLogger;
        private readonly ILogger<ProvisioningOrderCompletedHandler> provisioningOrderCompletedLogger;
        private readonly ILogger<ActivationOrderSentHandler> activationOrderSentLogger;
        private readonly ILogger<ActivationOrderCompletedHandler> activationOrderCompletedLogger;
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public MessageBusListenerBuilder(ILogger<OrderSentHandler> orderSentLogger, 
            ILogger<ProvisioningOrderCompletedHandler> provisioningOrderCompletedLogger,
            ILogger<ActivationOrderSentHandler> activationOrderSentLogger,
            ILogger<ActivationOrderCompletedHandler> activationOrderCompletedLogger,
            IMessageBus messageBus, 
            ISqsService sqsService,
            IRepository<Mobile> mobileRepository, 
            IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.orderSentLogger = orderSentLogger;
            this.provisioningOrderCompletedLogger = provisioningOrderCompletedLogger;
            this.activationOrderSentLogger = activationOrderSentLogger;
            this.activationOrderCompletedLogger = activationOrderCompletedLogger;
            this.messageBus = messageBus;
            this.sqsService = sqsService;
            this.mobileRepository = mobileRepository;
            this.getMobileByOrderIdQuery = getMobileByOrderIdQuery;
        }

        public IMessageBusListener Build()
        {
            var handler = new OrderSentHandler(orderSentLogger, mobileRepository, getMobileByOrderIdQuery);
            messageBus.Subscribe<OrderSentMessage, IHandlerAsync<OrderSentMessage>>(handler);

            var provisioningOrderCompletedHandler = new ProvisioningOrderCompletedHandler(provisioningOrderCompletedLogger, mobileRepository, getMobileByOrderIdQuery);
            messageBus.Subscribe<ProvisioningOrderCompletedMessage, IHandlerAsync<ProvisioningOrderCompletedMessage>>(provisioningOrderCompletedHandler);

            var activationOrderSentHandler = new ActivationOrderSentHandler(activationOrderSentLogger, mobileRepository, getMobileByOrderIdQuery);
            messageBus.Subscribe<ActivationOrderSentMessage, IHandlerAsync<ActivationOrderSentMessage>>(activationOrderSentHandler);

            var activationOrderCompletedHandler = new ActivationOrderCompletedHandler(activationOrderCompletedLogger, mobileRepository, getMobileByOrderIdQuery);
            messageBus.Subscribe<ActivationOrderCompletedMessage, IHandlerAsync<ActivationOrderCompletedMessage>>(activationOrderCompletedHandler);

            return new MessageBusListener(messageBus, sqsService, new MessageDeserializer());
        }
    }
}
