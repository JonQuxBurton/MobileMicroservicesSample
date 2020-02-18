using System.Collections.Generic;
using MobileOrderer.Api.Domain;

namespace MobileOrderer.Api.Data
{
    public interface IGetNewMobilesQuery
    {
        IEnumerable<Mobile> Get();
    }
}