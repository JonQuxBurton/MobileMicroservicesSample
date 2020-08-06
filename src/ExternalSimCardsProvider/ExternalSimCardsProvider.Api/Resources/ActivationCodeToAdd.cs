using System;

namespace ExternalSimCardsProvider.Api.Resources
{
    public class ActivationCodeToAdd
    {
        public string ActivationCode { get; set; }
        public Guid Reference { get; set; }
    }
}