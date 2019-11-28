using System;

namespace MobileOrderer.Api.Domain
{
    public interface IRepository<T> where T : AggregateRoot
    {
        T GetById(Guid id);
        void Save(T aggregateRoot);
    }
}
