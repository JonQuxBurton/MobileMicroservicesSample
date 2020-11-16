using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Utils.Enums;

namespace Mobiles.Api.Data
{
    public class GetNewCeasesQuery : IGetNewCeasesQuery
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly IEnumConverter enumConverter;
        private readonly MobilesContext mobilesContext;

        public GetNewCeasesQuery(MobilesContext mobilesContext, IEnumConverter enumConverter,
            IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get()
        {
            var ceaseMobileStateName = enumConverter.ToName<Mobile.MobileState>(Mobile.MobileState.ProcessingCease);
            var mobilesDataEntities = mobilesContext.Mobiles.Include(x => x.Orders)
                .Where(x => x.State == ceaseMobileStateName).ToList();
            var mobiles = new List<Mobile>();

            var newOrderStateName = enumConverter.ToName<Order.State>(Order.State.New);
            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var inProgressOrderDataEntity =
                    mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newOrderStateName);
                if (inProgressOrderDataEntity != null) mobiles.Add(new Mobile(dateTimeCreator, mobileDataEntity));
            }

            return mobiles;
        }
    }
}