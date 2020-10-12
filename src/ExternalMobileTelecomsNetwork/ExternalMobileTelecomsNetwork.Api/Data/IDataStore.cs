using DapperDataAccess;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public interface IDataStore
    {
        void Add(Order order);
        ITransaction BeginTransaction();
        void Complete(Guid reference);
        void Reject(Guid reference, string reason);

        ActivationCode GetActivationCode(string phoneNumber);
        bool UpdateActivationCode(ActivationCode existing);
        bool InsertActivationCode(ActivationCode activationCode);
        void Cease(string phoneNumber, Guid reference);
        Order GetByReference(Guid reference);
        Order GetByPhoneNumber(string phoneNumber, string status);
        IEnumerable<Order> GetAll();
    }
}