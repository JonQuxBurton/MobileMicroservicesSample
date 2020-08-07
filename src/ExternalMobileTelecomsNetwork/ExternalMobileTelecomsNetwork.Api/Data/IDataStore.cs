using DapperDataAccess;
using ExternalMobileTelecomsNetwork.Api.Resources;
using System;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public interface IDataStore
    {
        Order GetByReference(Guid reference);
        void Add(Order order);
        ITransaction BeginTransaction();
        void Cease(Guid reference);
        void Complete(Guid reference);

        ActivationCode GetActivationCode(Guid reference);
        bool UpdateActivationCode(ActivationCode existing);
        bool InsertActivationCode(ActivationCode activationCode);
    }
}