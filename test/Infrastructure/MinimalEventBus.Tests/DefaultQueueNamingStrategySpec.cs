using FluentAssertions;
using Xunit;

namespace MinimalEventBus.Tests
{
    public class DefaultQueueNamingStrategySpec
    {
        public class GetNameShould
        {
            private DefaultQueueNamingStrategy sut;

            public GetNameShould()
            {
                sut = new DefaultQueueNamingStrategy();
            }

            [Fact]
            public void ReturnTypeOfMessageWithMessageRemoved()
            {
                var expectedMessage = new TestMessage();

                var actual = sut.GetName(expectedMessage);

                actual.Should().Be("Test");
            }

            [Fact]
            public void ReturnNameOfTypeWithMessageRemoved()
            {
                var actual = sut.GetName(typeof(TestMessage));

                actual.Should().Be("Test");
            }

        }
    }
}
