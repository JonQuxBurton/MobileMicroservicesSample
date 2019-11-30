using System;

namespace Utils.DomainDrivenDesign
{
    public interface IRepository<T> where T : AggregateRoot
    {
        T GetById(Guid id);
        void Add(T aggregateRoot);
        void Update(T aggregateRoot);
    }
}
