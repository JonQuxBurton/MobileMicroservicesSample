using MobileOrderer.Api.Domain;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.Enums;

namespace MobileOrderer.Api.Data
{
    public class GetProcessingProvisioningMobilesQuery : IGetProcessingProvisioningMobilesQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;

        public GetProcessingProvisioningMobilesQuery(MobilesContext mobilesContext, IEnumConverter enumConverter)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
        }

        public IEnumerable<Mobile> Get()
        {
            var processingProvisioningStateName = enumConverter.ToName<Mobile.State>(Mobile.State.ProcessingProvisioning);
            var mobilesDataEntities = this.mobilesContext.Mobiles.Include(x => x.Orders).Where(x => x.State == processingProvisioningStateName).ToList();
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var newStateName = enumConverter.ToName<Mobile.State>(Order.State.New);

                var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newStateName);
                var inFlightOrder = new Order(inFlightOrderDataEntity);
                if (inFlightOrder != null)
                {
                    var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                    var orderHistory = orderHistoryDataEntities.Select(x => new Order(x));
                    mobiles.Add(new Mobile(mobileDataEntity, inFlightOrder, orderHistory));
                }
            }

            return mobiles;
        }
    }
}
