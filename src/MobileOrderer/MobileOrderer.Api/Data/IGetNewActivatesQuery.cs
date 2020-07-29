using MobileOrderer.Api.Domain;
using System.Collections.Generic;

namespace MobileOrderer.Api.Data
{
    public interface IGetNewActivatesQuery
    {
        IEnumerable<Mobile> Get();
    }
}