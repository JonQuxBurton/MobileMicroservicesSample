using ExternalSimCardsProvider.Api.Data;
using ExternalSimCardsProvider.Api.Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExternalSimCardsProvider.Api.Tests.Domain
{
    namespace ActivationCodeGeneratorSpec
    {
        public class GenerateShould
        {
            [Fact]
            public void ReturnActivationCode()
            {
                var maxId = 10;
                var expected = $"BAC{123 + maxId - 1}";
                var charCodesGeneratorMock = new Mock<IRandomCharCodesGenerator>();
                charCodesGeneratorMock.Setup(x => x.Generate(3))
                    .Returns(new[] { 66, 65, 67 });
                var ordersDataStoreMock = new Mock<IOrdersDataStore>();
                ordersDataStoreMock.Setup(x => x.GetMaxId())
                    .Returns(maxId);
                var sut = new ActivationCodeGenerator(charCodesGeneratorMock.Object, ordersDataStoreMock.Object);

                var actual = sut.Generate();

                actual.Should().Be(expected);
            }
        }
    }
}
