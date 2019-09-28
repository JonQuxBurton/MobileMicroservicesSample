using FluentAssertions;
using MinimalEventBus.JustSaying;
using Moq;
using Xunit;

namespace MinimalEventBus.Tests
{
    public class HandlerWrapperSpec
    {
        public class GetHandlerShould
        {
            [Fact]
            public void ReturnHandler()
            {
                var expectedHandlerMock = new Mock<IHandlerAsync<TestMessage>>();
                var sut = new HandlerWrapper<TestMessage>(expectedHandlerMock.Object);

                var actual = sut.GetHandler();

                actual.Should().Be(expectedHandlerMock.Object);
            }
        }
    }
}
