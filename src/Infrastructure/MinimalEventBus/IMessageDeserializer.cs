using Amazon.SQS.Model;
using MinimalEventBus.JustSaying;

namespace MinimalEventBus
{
    public interface IMessageDeserializer
    {
        T Deserialize<T>(Amazon.SQS.Model.Message busMessage) where T : JustSaying.Message;
    }
}