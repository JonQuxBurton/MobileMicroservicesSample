using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Messages;
using MobileTelecomsNetwork.EventHandlers.Services;

namespace MobileTelecomsNetwork.EventHandlers.Handlers
{
    public class ActivationRequestedHandler : IHandlerAsync<ActivationRequestedMessage>
    {
        private readonly ILogger<ActivationRequestedHandler> logger;
        private readonly IDataStore dataStore;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;
        private readonly IMessagePublisher messagePublisher;

        public ActivationRequestedHandler(ILogger<ActivationRequestedHandler> logger, 
            IDataStore dataStore,
            IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService,
            IMessagePublisher messagePublisher
            )
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.externalMobileTelecomsNetworkService = externalMobileTelecomsNetworkService;
            this.messagePublisher = messagePublisher;
        }

        public async Task<bool> Handle(ActivationRequestedMessage message)
        {
            this.logger.LogInformation($"Received [ActivationRequested] {message.Name} {message.ContactPhoneNumber}");

            using (var tx = dataStore.BeginTransaction())
            {
                this.dataStore.AddActivation(new ActivationOrder
                {
                    Name = message.Name,
                    MobileOrderId = message.MobileOrderId,
                    Status = "New"
                });
            }

            using (var tx = dataStore.BeginTransaction())
            {
                var result = await externalMobileTelecomsNetworkService.PostOrder(new ExternalMobileTelecomsNetworkOrder
                {
                    Reference = message.MobileOrderId,
                    Name = message.Name
                });

                if (!result)
                {
                    tx.Rollback();
                    return false;
                }

                this.dataStore.Sent(message.MobileOrderId);
                this.Publish(message.MobileOrderId);
            }

            return true;
        }

        private void Publish(Guid mobileGlobalId)
        {
            messagePublisher.PublishAsync(new ActivationOrderSentMessage
            {
                MobileOrderId = mobileGlobalId
            });
        }
    }
}
