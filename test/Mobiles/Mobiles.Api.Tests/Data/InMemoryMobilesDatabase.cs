using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;

namespace Mobiles.Api.Tests.Data
{
    public class InMemoryMobilesDatabase : IDisposable
    {
        private readonly InMemoryDatabase<MobilesContext> database;

        public InMemoryMobilesDatabase()
        {
            database = new InMemoryDatabase<MobilesContext>();
        }

        public DbConnection Connection => database.Connection;
        public DbContextOptions<MobilesContext> ContextOptions => database.ContextOptions;

        public void Dispose()
        {
            Connection.Dispose();
        }
        
        public void SetupAsEmpty()
        {
            using var context = new MobilesContext(database.ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        
        public void AddData(List<MobileDataEntity> data)
        {
            using var context = new MobilesContext(database.ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            foreach (var dataEntity in data) context.Mobiles.Add(dataEntity);

            context.SaveChanges();
        }
    }
}