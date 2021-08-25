using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Data;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    public class MobilesSharedFixture
    {
        public MobilesDataAccess DataAccess;

        public DbContextOptions<MobilesContext>
            ContextOptions =>
            new DbContextOptionsBuilder<MobilesContext>()
                .UseSqlServer(ConfigurationData.ConnectionString)
                .Options;

        private MobilesDataAccess CreateDataAccess(ITestOutputHelper output)
        {
            return new(output, ConfigurationData.ConnectionString);
        }

        public void Setup(ITestOutputHelper output)
        {
            DataAccess = CreateDataAccess(output);
        }
    }

    [CollectionDefinition("MobilesTests")]
    public class MobileRepositorySpecCollection : ICollectionFixture<MobilesSharedFixture>
    {
    }
}