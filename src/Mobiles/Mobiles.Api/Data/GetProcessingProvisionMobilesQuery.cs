using Mobiles.Api.Domain;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utils.DateTimes;
using Utils.Enums;

namespace Mobiles.Api.Data
{
    public class GetProcessingProvisionMobilesQuery : IGetProcessingProvisionMobilesQuery
    {
        private readonly MobilesContext mobilesContext;
        private readonly IEnumConverter enumConverter;
        private readonly IDateTimeCreator dateTimeCreator;

        public GetProcessingProvisionMobilesQuery(MobilesContext mobilesContext, IEnumConverter enumConverter, IDateTimeCreator dateTimeCreator)
        {
            this.mobilesContext = mobilesContext;
            this.enumConverter = enumConverter;
            this.dateTimeCreator = dateTimeCreator;
        }

        public IEnumerable<Mobile> Get()
        {
            var mobilesDataEntities = this.mobilesContext.Mobiles.Include(x => x.Orders).Where(x => x.State == Mobile.MobileState.ProcessingProvision.ToString()).ToList();
            var mobiles = new List<Mobile>();

            foreach (var mobileDataEntity in mobilesDataEntities)
            {
                var newStateName = enumConverter.ToName<Mobile.MobileState>(Order.State.New);

                var inFlightOrderDataEntity = mobileDataEntity.Orders.FirstOrDefault(x => x.State.Trim() == newStateName);
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
