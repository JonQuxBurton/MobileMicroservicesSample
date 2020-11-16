using System.Linq;

namespace Mobiles.Api.Data
{
    public class GetNextMobileIdQuery : IGetNextMobileIdQuery
    {
        private readonly MobilesContext mobilesContext;

        public GetNextMobileIdQuery(MobilesContext mobilesContext)
        {
            this.mobilesContext = mobilesContext;
        }

        public int Get()
        {
            return mobilesContext.Mobiles.Max(x => x.Id) + 1;
        }
    }
}
