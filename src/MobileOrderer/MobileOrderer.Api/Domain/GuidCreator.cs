using System;

namespace MobileOrderer.Api.Domain
{
    public class GuidCreator : IGuidCreator
    {
        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}
