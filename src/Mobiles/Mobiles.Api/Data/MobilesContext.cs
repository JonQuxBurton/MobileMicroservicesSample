using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Domain;

namespace Mobiles.Api.Data
{
    public class MobilesContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<MobileDataEntity> Mobiles{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<MobileDataEntity>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<OrderDataEntity>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("getdate()");
        }

        public MobilesContext(DbContextOptions<MobilesContext> options) : base(options) 
        { }
    }
}
