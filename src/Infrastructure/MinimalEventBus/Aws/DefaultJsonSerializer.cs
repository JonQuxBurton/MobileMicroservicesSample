namespace MinimalEventBus.Aws
{
    public class DefaultJsonSerializer : IJsonSerializer
    {
        public string Serialize(object message)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(message);
        }
    }
}
