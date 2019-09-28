using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace DapperDataAccess
{
    public interface ITransaction : IDisposable
    {
        DbTransaction Get();
        void Rollback();
    }
}