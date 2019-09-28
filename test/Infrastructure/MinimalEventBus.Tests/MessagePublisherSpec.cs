using MinimalEventBus.Aws;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace MinimalEventBus.Tests
{
    public partial class MessagePublisherSpec
    {
        public class MessagePublisherShould
        {
            [Fact]
            public async void PublishMessageToMessageBus()
            {
                var expectedMessage = new TestMessage { Name = "Expected" };
                var expectedQueueName = "ExpectedQueue";
                var messageBusMock = new Mock<IMessageBus>();
                var snsServiceMock = new Mock<ISnsService>();
                messageBusMock.Setup(x => x.SetupQueue(expectedMessage))
                    .Returns(Task.FromResult(expectedQueueName));

                var sut = new MessagePublisher(messageBusMock.Object, snsServiceMock.Object);

                await sut.PublishAsync(expectedMessage);

                snsServiceMock.Verify(x => x.PublishAsync(expectedQueueName, expectedMessage));
            }
        }
    }
}
