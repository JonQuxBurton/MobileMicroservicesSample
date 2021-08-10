using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using static Mobiles.Api.Domain.Order;

namespace Mobiles.Api.Data
{
    public class GetNewActivatesQuery : IGetNewActivatesQuery
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly MobilesContext mobilesContext;

        public GetNewActivatesQuery(MobilesContext mobilesContext,
            IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get()
        {
            return mobilesContext.Mobiles
                .Include(x => x.Orders)
                .Where(x => x.Orders.Any(y =>
                    y.Type == OrderType.Activate.ToString() && y.State == Mobile.MobileState.New.ToString()))
                .Select(x => new Mobile(dateTimeCreator, x));
        }
    }
}