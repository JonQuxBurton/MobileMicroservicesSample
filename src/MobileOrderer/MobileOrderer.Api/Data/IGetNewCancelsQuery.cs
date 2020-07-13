using System.Collections.Generic;
using MobileOrderer.Api.Domain;

namespace MobileOrderer.Api.Data
{
    public interface IGetNewCancelsQuery
    {
        IEnumerable<Mobile> Get();
    }
}