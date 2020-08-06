using System;
using System.Collections.Generic;

namespace ExternalSimCardsProvider.Api.Domain
{
    public class RandomCharCodesGenerator : IRandomCharCodesGenerator
    {
        private readonly Random random;

        public RandomCharCodesGenerator()
        {
            random = new Random();
        }

        public IEnumerable<int> Generate(int quantity)
        {
            for (var i = 0; i < quantity; i++)
                yield return random.Next(65, 91);
        }
    }
}