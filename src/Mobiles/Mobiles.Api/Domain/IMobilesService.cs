using System;
using System.Collections.Generic;
using Mobiles.Api.Resources;

namespace Mobiles.Api.Domain
{
    public interface IMobilesService
    {
        Mobile Activate(Guid id, ActivateRequest activateRequest);
        Mobile Cease(Guid id);
        IEnumerable<string> GetAvailablePhoneNumbers();
    }
}