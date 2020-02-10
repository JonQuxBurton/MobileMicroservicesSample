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
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGetMobileByOrderIdQuery getMobileByOrderIdQuery;

        public MessageBusListenerBuilder(ILogger<OrderSentHandler> orderSentLogger, 
            ILogger<ProvisioningOrderCompletedHandler> provisioningOrderCompletedLogger,
            IMessageBus messageBus, 
            ISqsService sqsService,
            IRepository<Mobile> mobileRepository, 
            IGetMobileByOrderIdQuery getMobileByOrderIdQuery)
        {
            this.orderSentLogger = orderSentLogger;
            this.provisioningOrderCompletedLogger = provisioningOrderCompletedLogger;
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

            return new MessageBusListener(messageBus, sqsService, new MessageDeserializer());
        }
    }
}
