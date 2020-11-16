using Mobiles.Api.Domain;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.DateTimes;
using Utils.Enums;
using static Mobiles.Api.Domain.Order;

namespace Mobiles.Api.Data
{
    public class GetNewActivatesQuery : IGetNewActivatesQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;
        private readonly IDateTimeCreator dateTimeCreator;

        public GetNewActivatesQuery(MobilesContext mobilesContext, 
            IEnumConverter enumConverter, 
            IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get()
        {
            var newStateName = enumConverter.ToName<Mobile.MobileState>(Mobile.MobileState.New);
            var activateOrderType = enumConverter.ToName<OrderType>(OrderType.Activate);
            var mobilesDataEntities = this.mobilesContext.Mobiles
                .Include(x => x.Orders)
                .Where(x => x.Orders.Any(y => y.Type == activateOrderType && y.State == newStateName))
                .ToList();

            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newStateName);
                var inFlightOrder = new Order(inFlightOrderDataEntity);
                if (inFlightOrder != null)
                {
                    //var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                    var orderHistory = mobileDataEntity.Orders.Select(x => new Order(x));

                    mobiles.Add(new Mobile(dateTimeCreator, mobileDataEntity));
                }
            }

            return mobiles;
        }
    }
}
