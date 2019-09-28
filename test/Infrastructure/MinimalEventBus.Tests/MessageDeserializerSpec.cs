using Newtonsoft.Json;
using Xunit;

namespace MinimalEventBus.Tests
{
    public class MessageDeserializerSpec
    {
        public class DeserializeShould
        {
            [Fact]
            public void DeserializeMessage()
            {
                var testMessage = new TestMessage();
                testMessage.Name = "Test";
                var testMessageJson = JsonConvert.SerializeObject(testMessage);

                BusBodyMessage busBodyMessage = new BusBodyMessage();
                busBodyMessage.Message = testMessageJson;
                var busBodyJson = JsonConvert.SerializeObject(busBodyMessage);

                var message = new Amazon.SQS.Model.Message
                {
                    Body = busBodyJson
                };

                var sut = new MessageDeserializer();

                var actual = sut.Deserialize<TestMessage>(message);

                actual.Name = "ExpectedMessage";
            }
        }
    }
}
