using MinimalEventBus.Aws;
using Moq;
using Xunit;
using Amazon.SQS.Model;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers;
using FluentAssertions;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;

namespace MinimalEventBus.Tests
{
    public class MessageBusSpec
    {
        public class SetupQueueShould
        {
            private Mock<IQueueNamingStrategy> queueNamingStrategyMock;
            private Mock<ISnsService> snsServiceMock;
            private Mock<ISqsService> sqsServiceMock;

            private MessageBus sut;

            private string expectedQueueName;
            private TestMessage expectedMessage;
            private string expectedQueueUrl;
            private string expectedErrorQueueName;
            private string expectedErrorQueueUrl;
            private string expectedTopicArn;

            public SetupQueueShould()
            {
                expectedQueueName = "Test";
                expectedMessage = new TestMessage();

                expectedQueueUrl = "URL:QueueName";
                expectedErrorQueueName = $"{expectedQueueName}_error";
                expectedErrorQueueUrl = "URL:ErrorQueueName";
                expectedTopicArn = "ARN:QueueName";

                queueNamingStrategyMock = new Mock<IQueueNamingStrategy>();
                snsServiceMock = new Mock<ISnsService>();
                sqsServiceMock = new Mock<ISqsService>();

                queueNamingStrategyMock.Setup(x => x.GetName(expectedMessage)).Returns(expectedQueueName);
                sqsServiceMock.Setup(x => x.CreateQueue(expectedQueueName)).Returns(Task.FromResult(new CreateQueueResponse()
                {
                    QueueUrl = expectedQueueUrl
                }));
                sqsServiceMock.Setup(x => x.CreateQueue(expectedErrorQueueName)).Returns(Task.FromResult(new CreateQueueResponse()
                {
                    QueueUrl = expectedErrorQueueUrl
                }));
                this.snsServiceMock.Setup(x => x.CreateTopic(expectedQueueName)).Returns(Task.FromResult(new CreateTopicResponse()
                {
                    TopicArn = expectedTopicArn
                }));

                sut = new MessageBus(snsServiceMock.Object, sqsServiceMock.Object, queueNamingStrategyMock.Object);
            }

            [Fact]
            public async void ReturnQueueName()
            {
                var actual = await sut.SetupQueue(expectedMessage);

                actual.Should().Be(expectedQueueName);
            }

            [Fact]
            public async void CreateQueue()
            {
                await sut.SetupQueue(expectedMessage);

                this.sqsServiceMock.Verify(x => x.CreateQueue(expectedQueueName));
            }

            [Fact]
            public async void CreateErrorQueue()
            {
                await sut.SetupQueue(expectedMessage);

                this.sqsServiceMock.Verify(x => x.CreateQueue(expectedErrorQueueName));
            }

            [Fact]
            public async void SubscribeQueueToTopic()
            {
                await sut.SetupQueue(expectedMessage);

                this.snsServiceMock.Verify(x => x.SubscribeQueueAsync(expectedTopicArn, expectedQueueUrl));
            }

            [Fact]
            public async void AddQueueToQueues()
            {
                await sut.SetupQueue(expectedMessage);

                var actual = sut.Queues[expectedQueueName];

                actual.Should().NotBeNull();
                actual.MessageType.Should().Be(expectedMessage.GetType());
                actual.Url.Should().Be(expectedQueueUrl);
            }

            [Fact]
            public async void AddErrorQueueToQueues()
            {
                await sut.SetupQueue(expectedMessage);

                var actual = sut.Queues[expectedErrorQueueName];

                actual.Should().NotBeNull();
                actual.MessageType.Should().Be(expectedMessage.GetType());
                actual.Url.Should().Be(expectedErrorQueueUrl);
            }
        }

        public class SubscribeShould
        {
            private Mock<IQueueNamingStrategy> queueNamingStrategyMock;
            private Mock<ISnsService> snsServiceMock;
            private Mock<ISqsService> sqsServiceMock;

            MessageBus sut;

            public SubscribeShould()
            {
                queueNamingStrategyMock = new Mock<IQueueNamingStrategy>();
                snsServiceMock = new Mock<ISnsService>();
                sqsServiceMock = new Mock<ISqsService>();

                sut = new MessageBus(snsServiceMock.Object, sqsServiceMock.Object, queueNamingStrategyMock.Object);
            }

            [Fact]
            public void AddQueue()
            {
                var expectedQueueName = "TestQueue";
                var expectedErrorQueueName = $"{expectedQueueName}_error";
                var expectedMessage = new TestMessage { Name = "TestMessage" };
                var expectedQueueUrl = "URL:TestQueue";
                var createQueueResponse = new CreateQueueResponse { QueueUrl = expectedQueueUrl };
                queueNamingStrategyMock.Setup(x => x.GetName(typeof(TestMessage))).Returns(expectedQueueName);
                sqsServiceMock.Setup(x => x.GetQueueUrl(expectedQueueName)).Returns(createQueueResponse.QueueUrl);

                sut.Subscribe<TestMessage, IHandlerAsync<TestMessage>>(new TestHandler());

                var actual = sut.Queues[expectedQueueName];

                actual.Should().NotBeNull();
                actual.MessageType.Should().Be(expectedMessage.GetType());
                actual.Url.Should().Be(expectedQueueUrl);
            }

            [Fact]
            public void AddHandlerResolver()
            {
                var expectedQueueName = "TestQueue";
                var expectedErrorQueueName = $"{expectedQueueName}_error";
                var expectedMessage = new TestMessage { Name = "TestMessage" };
                var createQueueResponse = new CreateQueueResponse { QueueUrl = "URL:TestQueue" };
                queueNamingStrategyMock.Setup(x => x.GetName(typeof(TestMessage))).Returns(expectedQueueName);
                sqsServiceMock.Setup(x => x.GetQueueUrl(expectedQueueName)).Returns(createQueueResponse.QueueUrl);

                sut.Subscribe<TestMessage, IHandlerAsync<TestMessage>>(new TestHandler());

                var expected = sut.GetHandler<TestMessage>();

                expected.Should().NotBeNull();
            }
        }

        public class GetHandlerShould
        {
            private Mock<IQueueNamingStrategy> queueNamingStrategyMock;
            private Mock<ISnsService> snsServiceMock;
            private Mock<ISqsService> sqsServiceMock;

            MessageBus sut;

            public GetHandlerShould()
            {
                queueNamingStrategyMock = new Mock<IQueueNamingStrategy>();
                snsServiceMock = new Mock<ISnsService>();
                sqsServiceMock = new Mock<ISqsService>();

                sut = new MessageBus(snsServiceMock.Object, sqsServiceMock.Object, queueNamingStrategyMock.Object);
            }

            [Fact]
            public void ReturnMatchingHandler()
            {
                var expectedQueueName = "TestQueue";
                var expectedErrorQueueName = $"{expectedQueueName}_error";
                var expectedMessage = new TestMessage { Name = "TestMessage" };
                var expectedQueueUrl = "URL:TestQueue";
                var createQueueResponse = new CreateQueueResponse { QueueUrl = expectedQueueUrl };
                queueNamingStrategyMock.Setup(x => x.GetName(typeof(TestMessage))).Returns(expectedQueueName);
                sqsServiceMock.Setup(x => x.GetQueueUrl(expectedQueueName)).Returns(createQueueResponse.QueueUrl);

                var handlerMock = new Mock<IHandlerAsync<TestMessage>>();
                sut.Subscribe<TestMessage, IHandlerAsync<TestMessage>>(handlerMock.Object);

                var actual = sut.GetHandler<TestMessage>();

                actual.Should().NotBeNull();
            }

            [Fact]
            public void ReturnNullWhenNoMatchingHandler()
            {
                var actual = sut.GetHandler<TestMessage>();

                actual.Should().BeNull();
            }
        }
    }
}
