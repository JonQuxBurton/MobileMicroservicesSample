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
            logger.LogInformation($"Received [ActivationRequested] {message.Name} {message.ContactPhoneNumber}");

            try
            {

                using (var tx = dataStore.BeginTransaction())
                {
                    dataStore.Add(new Order
                    {
                        Name = message.Name,
                        MobileOrderId = message.MobileOrderId,
                        Status = "New",
                        Type = "Activate",
                    });
                }

                using (var tx = dataStore.BeginTransaction())
                {
                    var result = await externalMobileTelecomsNetworkService.PostOrder(new ExternalMobileTelecomsNetworkOrder
                    {
                        Reference = message.MobileOrderId
                    });

                    if (!result)
                    {
                        tx.Rollback();
                        return false;
                    }

                    dataStore.Sent(message.MobileOrderId);
                    Publish(message.MobileOrderId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return false;
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
