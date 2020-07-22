using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using MobileTelecomsNetwork.EventHandlers.Messages;

namespace MobileTelecomsNetwork.EventHandlers.Handlers
{
    public class ActivationRequestedHandler : IHandlerAsync<ActivationRequestedMessage>
    {
        private readonly ILogger<ActivationRequestedHandler> logger;
        private readonly IDataStore dataStore;
        private readonly IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService;
        private readonly IMessagePublisher messagePublisher;
        private readonly IMonitoring monitoring;

        public ActivationRequestedHandler(ILogger<ActivationRequestedHandler> logger,
            IDataStore dataStore,
            IExternalMobileTelecomsNetworkService externalMobileTelecomsNetworkService,
            IMessagePublisher messagePublisher,
            IMonitoring monitoring
            )
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.externalMobileTelecomsNetworkService = externalMobileTelecomsNetworkService;
            this.messagePublisher = messagePublisher;
            this.monitoring = monitoring;
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
                        monitoring.ActivateOrderFailed();
                        return false;
                    }

                    dataStore.Sent(message.MobileOrderId);
                    Publish(message.MobileOrderId);
                    monitoring.ActivateOrderSent();
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
