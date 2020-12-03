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
            var ids =  mobilesContext.Mobiles.Select(x => x.Id);

            if (!ids.Any())
                return 1;

            return ids.Max() + 1;
        }
    }
}
