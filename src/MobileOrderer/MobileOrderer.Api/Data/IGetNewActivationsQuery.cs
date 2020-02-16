using MobileOrderer.Api.Domain;
using System.Collections.Generic;

namespace MobileOrderer.Api.Data
{
    public interface IGetNewActivationsQuery
    {
        IEnumerable<Mobile> GetNew();
    }
}