using System;

namespace Utils.Guids
{
    public class GuidCreator : IGuidCreator
    {
        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}
