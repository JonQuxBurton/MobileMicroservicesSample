using MinimalEventBus.JustSaying;
using Newtonsoft.Json;

namespace MinimalEventBus
{
    public class MessageDeserializer : IMessageDeserializer
    {
        public T Deserialize<T>(Amazon.SQS.Model.Message busMessage) where T : Message
        {
            
            var busBodyMessage = JsonConvert.DeserializeObject<BusBodyMessage>(busMessage.Body);
            var message = JsonConvert.DeserializeObject<T>(busBodyMessage.Message);
            return message;
        }
    }
}
