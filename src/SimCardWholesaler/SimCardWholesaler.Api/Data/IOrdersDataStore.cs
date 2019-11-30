﻿using DapperDataAccess;
using System;

namespace SimCardWholesaler.Api.Data
{
    public interface IOrdersDataStore
    {
        void Add(Order order);
        Order GetByReference(Guid globalId);
        ITransaction BeginTransaction();
    }
}