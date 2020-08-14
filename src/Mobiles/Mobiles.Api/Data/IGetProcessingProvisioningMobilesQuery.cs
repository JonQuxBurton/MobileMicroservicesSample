using Mobiles.Api.Domain;
using System.Collections.Generic;

namespace Mobiles.Api.Data
{
    public interface IGetProcessingProvisioningMobilesQuery
    {
        IEnumerable<Mobile> Get();
    }
}