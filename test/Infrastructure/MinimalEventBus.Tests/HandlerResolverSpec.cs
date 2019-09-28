using FluentAssertions;
using MinimalEventBus.JustSaying;
using Moq;
using Xunit;

namespace MinimalEventBus.Tests
{
    public partial class HandlerResolverSpec
    {
        public class ResolveShould
        {
            [Fact]
            public void ReturnHandler()
            {
                var sut = new HandlerResolver();
                var handlerMock = new Mock<IHandlerAsync<TestMessage>>();
                var handlerWrapper = new HandlerWrapper<TestMessage>(handlerMock.Object);

                sut.Add(handlerWrapper);

                var actual = sut.Resolve<TestMessage>();

                actual.Should().Be(handlerMock.Object);

            }

            [Fact]
            public void ReturnNullWhenHandlerDoesNotMatch()
            {
                var sut = new HandlerResolver();
                var handlerMock = new Mock<IHandlerAsync<TestMessage>>();
                var handlerWrapper = new HandlerWrapper<TestMessage>(handlerMock.Object);

                sut.Add(handlerWrapper);

                var actual = sut.Resolve<AnotherTestMessage>();

                actual.Should().BeNull();
            }
        }
    }
}
