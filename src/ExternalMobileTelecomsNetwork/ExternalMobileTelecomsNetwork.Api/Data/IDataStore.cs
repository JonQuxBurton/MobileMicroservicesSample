using DapperDataAccess;
using ExternalMobileTelecomsNetwork.Api.Resources;
using System;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public interface IDataStore
    {
        void Add(Order order);
        void AddActivationCode(ActivationCodeToAdd activationCodeToAdd);
        ITransaction BeginTransaction();
        void Cease(Guid reference);
        void Complete(Guid reference);
        Order GetByReference(Guid reference);
    }
}