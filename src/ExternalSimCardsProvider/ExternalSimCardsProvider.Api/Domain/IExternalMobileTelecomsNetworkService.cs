using System;
using System.Threading.Tasks;

namespace ExternalSimCardsProvider.Api.Domain
{
    public interface IExternalMobileTelecomsNetworkService
    {
        Task<bool> PostActivationCode(string phoneNumber, string activationCode);
    }
}