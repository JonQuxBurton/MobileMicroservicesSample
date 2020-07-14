using System.Text.Json;

namespace EndToEndApiLevelTests
{
    public class SnapshotFactoryBase
    {
        protected T Snapshot<T>(T original) where T : class
        {
            if (original == null)
                return null;

            var serialized = JsonSerializer.Serialize(original);
            return JsonSerializer.Deserialize<T>(serialized);
        }
    }
}
