using DapperDataAccess;
using System;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public interface IDataStore
    {
        void Add(Order order);
        ITransaction BeginTransaction();
        void Complete(Guid mobileReference);

        ActivationCode GetActivationCode(string phoneNumber);
        bool UpdateActivationCode(ActivationCode existing);
        bool InsertActivationCode(ActivationCode activationCode);
        void Cease(Guid mobileReference, string phoneNumber);
        Order GetByReference(Guid mobileReference, string status = "New");
    }
}