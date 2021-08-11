using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Domain;
using Utils.DateTimes;

namespace Mobiles.Api.Data
{
    public class GetNewProvisionsQuery : IGetNewProvisionsQuery
    {
        private readonly IDateTimeCreator dateTimeCreator;
        private readonly MobilesContext mobilesContext;

        public GetNewProvisionsQuery(MobilesContext mobilesContext, IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get()
        {
            var mobilesDataEntities = mobilesContext.Mobiles.Include(x => x.Orders)
                .Where(x => x.State == Mobile.MobileState.New.ToString())
                .ToList();
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var newOrderDataEntity =
                    mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == Order.State.New.ToString());

                if (newOrderDataEntity != null) mobiles.Add(new Mobile(dateTimeCreator, mobileDataEntity));
            }

            return mobiles;
        }
    }
}