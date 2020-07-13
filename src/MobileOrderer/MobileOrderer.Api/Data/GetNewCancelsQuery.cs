using MobileOrderer.Api.Domain;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.Enums;

namespace MobileOrderer.Api.Data
{
    public class GetNewCancelsQuery : IGetNewCancelsQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;

        public GetNewCancelsQuery(MobilesContext mobilesContext, IEnumConverter enumConverter)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
        }

        public IEnumerable<Mobile> Get()
        {
            var cancelMobileStateName = enumConverter.ToName<Mobile.State>(Mobile.State.ProcessingCease);
            var mobilesDataEntities = this.mobilesContext.Mobiles.Include(x => x.Orders).Where(x => x.State == cancelMobileStateName).ToList();
            var mobiles = new List<Mobile>();

            var newOrderStateName = enumConverter.ToName<Order.State>(Order.State.New);
            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newOrderStateName);
                if (inFlightOrderDataEntity != null)
                {
                    var inFlightOrder = new Order(inFlightOrderDataEntity);
                    var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                    var orderHistory = orderHistoryDataEntities.Select(x => new Order(x));
                    mobiles.Add(new Mobile(mobileDataEntity, inFlightOrder, orderHistory));
                }
            }

            return mobiles;
        }
    }
}
