using ExternalSimCardsProvider.Api.Data;
using System;
using System.Linq;

namespace ExternalSimCardsProvider.Api.Domain
{
    public class ActivationCodeGenerator : IActivationCodeGenerator
    {
        private const int NumberOfLetters = 3;
        private const int StartingNumber = 123;
        private const int WrapAroundAt = 1000;

        private readonly IRandomCharCodesGenerator randomCharCodesGenerator;
        private readonly IOrdersDataStore ordersDataStore;

        public ActivationCodeGenerator(IRandomCharCodesGenerator randomCharCodesGenerator, IOrdersDataStore ordersDataStore)
        {
            this.randomCharCodesGenerator = randomCharCodesGenerator;
            this.ordersDataStore = ordersDataStore;
        }

        public string Generate()
        {
            var maxId = ordersDataStore.GetMaxId();
            var numericPart = (maxId - 1 + StartingNumber) % WrapAroundAt;
            var charCodes = randomCharCodesGenerator.Generate(NumberOfLetters);
            var letters = string.Join("", charCodes.Select(x => Convert.ToChar(x)));

            return $"{letters}{numericPart}";
        }
    }
}