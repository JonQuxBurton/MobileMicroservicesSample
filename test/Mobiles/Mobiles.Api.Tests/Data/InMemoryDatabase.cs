using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Mobiles.Api.Tests.Data
{
    public class InMemoryDatabase<T> : IDisposable where T : DbContext
    {
        public readonly DbConnection Connection;
        public readonly DbContextOptions<T> ContextOptions;

        public InMemoryDatabase()
        {
            ContextOptions = new DbContextOptionsBuilder<T>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            Connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        public void Dispose()
        {
            Connection.Dispose();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.CreateFunction("getdate", () => DateTime.Now);

            connection.Open();

            return connection;
        }
    }
}