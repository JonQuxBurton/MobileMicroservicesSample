using System.Collections.Generic;

namespace ExternalSimCardsProvider.Api.Domain
{
    public interface IRandomCharCodesGenerator
    {
        IEnumerable<int> Generate(int quantity);
    }
}