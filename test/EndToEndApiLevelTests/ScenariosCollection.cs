using Xunit;

namespace EndToEndApiLevelTests
{
    [CollectionDefinition("Scenarios collection")]
    public class ScenariosCollection : ICollectionFixture<ScenariosFixture>
    {
    }
}
