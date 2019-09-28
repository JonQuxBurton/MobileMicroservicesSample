using Amazon.SQS.Model;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MinimalEventBus.Tests
{
    public class MessageBusListenerSpec
    {
        public class StartListeningShould
        {
            [Fact]
            public void ReceiveMessagesForQueue()
            {
                var expectedQueueUrl = "QueueUrl";
                var queues = new Dictionary<string, QueueInfo>();
                queues.Add("", new QueueInfo(typeof(TestMessage), expectedQueueUrl));

                var messageBusMock = new Mock<IMessageBus>();
                var sqsServiceMock = new Mock<ISqsService>();
                var messageDeserializerMock = new Mock<IMessageDeserializer>();
                messageBusMock.Setup(x => x.Queues).Returns(queues);

                var sut = new MessageBusListener(messageBusMock.Object, sqsServiceMock.Object, messageDeserializerMock.Object);

                sut.StartListening();

                sqsServiceMock.Verify(x => x.ReceiveMessageAsync(expectedQueueUrl));
            }
        }

        public class CheckForMessagesShould
        {
            private MessageBusListener sut;
            private QueueInfo queueInfo;
            private Mock<IHandlerAsync<TestMessage>> handlerMock;
            private TestMessage expectedMessage;
            private Mock<ISqsService> sqsServiceMock;
            private string expectedQueueUrl;
            private Amazon.SQS.Model.Message busMessage;

            public CheckForMessagesShould()
            {
                expectedQueueUrl = "QueueUrl";
                queueInfo = new QueueInfo(typeof(TestMessage), expectedQueueUrl);
                var queues = new Dictionary<string, QueueInfo>();
                queues.Add("", queueInfo);

                var messageBusMock = new Mock<IMessageBus>();
                sqsServiceMock = new Mock<ISqsService>();
                var messageDeserializerMock = new Mock<IMessageDeserializer>();
                handlerMock = new Mock<IHandlerAsync<TestMessage>>();

                messageBusMock.Setup(x => x.Queues).Returns(queues);
                messageBusMock.Setup(x => x.GetHandler<TestMessage>()).Returns(handlerMock.Object);

                expectedMessage = new TestMessage();
                var response = new Amazon.SQS.Model.ReceiveMessageResponse();
                busMessage = new Amazon.SQS.Model.Message();
                busMessage.ReceiptHandle = "Recipt001";
                response.Messages.Add(busMessage);
                sqsServiceMock.Setup(x => x.ReceiveMessageAsync(expectedQueueUrl)).Returns(Task.FromResult(response));
                messageDeserializerMock.Setup(x => x.Deserialize<TestMessage>(busMessage)).Returns(expectedMessage);

                sut = new MessageBusListener(messageBusMock.Object, sqsServiceMock.Object, messageDeserializerMock.Object);
            }

            [Fact]
            public async void CallHandlerWithMessage()
            {
                await sut.CheckForMessages<TestMessage>(queueInfo);

                handlerMock.Verify(x => x.Handle(expectedMessage));
            }

            [Fact]
            public async void DeletesMessageWhenHandledSucceeds()
            {
                handlerMock.Setup(x => x.Handle(expectedMessage)).Returns(Task.FromResult(true));
                sqsServiceMock.Setup(x => x.DeleteMessageAsync(expectedQueueUrl, busMessage.ReceiptHandle))
                    .Returns(Task.FromResult(new DeleteMessageResponse() { HttpStatusCode = System.Net.HttpStatusCode.OK }));

                await sut.CheckForMessages<TestMessage>(queueInfo);

                sqsServiceMock.Verify(x => x.DeleteMessageAsync(expectedQueueUrl, busMessage.ReceiptHandle));
            }

            [Fact]
            public async void NotDeletesMessageWhenHandleFails()
            {
                handlerMock.Setup(x => x.Handle(expectedMessage)).Returns(Task.FromResult(false));
                sqsServiceMock.Setup(x => x.DeleteMessageAsync(expectedQueueUrl, busMessage.ReceiptHandle))
                    .Returns(Task.FromResult(new DeleteMessageResponse() { HttpStatusCode = System.Net.HttpStatusCode.OK }));

                await sut.CheckForMessages<TestMessage>(queueInfo);

                sqsServiceMock.Verify(x => x.DeleteMessageAsync(expectedQueueUrl, busMessage.ReceiptHandle), Times.Never);
            }
        }
    }
}
