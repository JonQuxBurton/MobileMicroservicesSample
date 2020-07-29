﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Messages;
using System;
using System.Threading.Tasks;
using Utils.DomainDrivenDesign;

namespace MobileOrderer.Api.Handlers
{
    public class CeaseOrderSentHandler : IHandlerAsync<CeaseOrderSentMessage>
    {
        private readonly ILogger<CeaseOrderSentHandler> logger;
        private readonly IServiceProvider serviceProvider;

        public CeaseOrderSentHandler(ILogger<CeaseOrderSentHandler> logger,
            IServiceProvider serviceProvider
            )
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public Task<bool> Handle(CeaseOrderSentMessage message)
        {
            var messageName = message.GetType().Name;
            logger.LogInformation($"Received [{messageName}] MobileOrderId={message.MobileOrderId}");

            try
            {

                using var scope = serviceProvider.CreateScope();
                var getMobileByOrderIdQuery = scope.ServiceProvider.GetRequiredService<IGetMobileByOrderIdQuery>();
                var mobileRepository = scope.ServiceProvider.GetRequiredService<IRepository<Mobile>>();

                var mobile = getMobileByOrderIdQuery.Get(message.MobileOrderId);
                mobile.OrderSent();
                mobileRepository.Update(mobile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while processing {messageName}");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
