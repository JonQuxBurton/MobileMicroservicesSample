using System.Reflection;

namespace EndToEndApiLevelTests
{
    public class SnapshotFactoryBase
    {
        protected T Snapshot<T>(T original) where T: class
        {
            if (original == null)
                return null;

            MethodInfo method = original.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)method.Invoke(original, null);
        }
    }
}
