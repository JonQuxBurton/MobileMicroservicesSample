using System.Data.Common;

namespace DapperDataAccess
{
    public class Transaction : ITransaction
    {
        private DbTransaction currentTransaction;
        private readonly DbConnection connection;

        public Transaction(DbConnection connection)
        {
            this.connection = connection;
            this.connection.Open();
            currentTransaction = this.connection.BeginTransaction();
        }

        public void Dispose()
        {
            currentTransaction.Commit();
            connection.Close();
        }

        public DbTransaction Get()
        {
            return currentTransaction;
        }

        public void Rollback()
        {
            currentTransaction.Rollback();
        }
    }
}
