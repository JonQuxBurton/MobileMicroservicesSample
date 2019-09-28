namespace MinimalEventBus.Aws
{
    public interface IJsonSerializer
    {
        string Serialize(object message);
    }
}