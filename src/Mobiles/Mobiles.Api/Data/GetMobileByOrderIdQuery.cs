using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Domain;
using Utils.DateTimes;

namespace Mobiles.Api.Data
{
    public class GetMobileByOrderIdQuery : IGetMobileByOrderIdQuery
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly MobilesContext mobilesContext;

        public GetMobileByOrderIdQuery(MobilesContext mobilesContext, IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.dateTimeCreator = dateTimeCreator;
        }

        public Mobile Get(Guid orderId)
        {
            var mobileDataEntity = mobilesContext.Mobiles
                .Include(x => x.Orders)
                .FirstOrDefault(x => x.Orders.Any(y => y.GlobalId == orderId));

            if (mobileDataEntity == null)
                return null;

            return new Mobile(dateTimeCreator, mobileDataEntity);
        }
    }
}