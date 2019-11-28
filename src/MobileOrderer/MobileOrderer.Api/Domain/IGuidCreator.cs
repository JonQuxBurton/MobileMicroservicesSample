using System;

namespace MobileOrderer.Api.Domain
{
    public interface IGuidCreator
    {
        Guid Create();
    }
}