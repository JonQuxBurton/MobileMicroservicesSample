using Mobiles.Api.Domain;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.DateTimes;
using Utils.Enums;

namespace Mobiles.Api.Data
{
    public class GetNewCeasesQuery : IGetNewCeasesQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;
        private readonly IDateTimeCreator dateTimeCreator;

        public GetNewCeasesQuery(MobilesContext mobilesContext, IEnumConverter enumConverter, IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get()
        {
            var ceaseMobileStateName = enumConverter.ToName<Mobile.MobileState>(Mobile.MobileState.ProcessingCease);
            var mobilesDataEntities = this.mobilesContext.Mobiles.Include(x => x.Orders).Where(x => x.State == ceaseMobileStateName).ToList();
            var mobiles = new List<Mobile>();

            var newOrderStateName = enumConverter.ToName<Order.State>(Order.State.New);
            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newOrderStateName);
                if (inFlightOrderDataEntity != null)
                {
                    var inFlightOrder = new Order(inFlightOrderDataEntity);
                    //var orderHistoryDataEntities = mobileDataEntity.Orders.Except(new[] { inFlightOrderDataEntity });
                    var orderHistory = mobileDataEntity.Orders.Select(x => new Order(x));
                    mobiles.Add(new Mobile(dateTimeCreator, mobileDataEntity));
                }
            }

            return mobiles;
        }
    }
}
