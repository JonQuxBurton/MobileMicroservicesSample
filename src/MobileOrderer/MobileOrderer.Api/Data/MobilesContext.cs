using Microsoft.EntityFrameworkCore;
using MobileOrderer.Api.Domain;

namespace MobileOrderer.Api.Data
{
    public class MobilesContext : DbContext
    {
        public DbSet<MobileDataEntity> Mobiles{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
