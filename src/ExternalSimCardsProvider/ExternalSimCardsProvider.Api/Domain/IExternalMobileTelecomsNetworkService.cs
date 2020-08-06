using System.Threading.Tasks;

namespace ExternalSimCardsProvider.Api.Domain
{
    public interface IExternalMobileTelecomsNetworkService
    {
        Task<bool> PostActivationCode(System.Guid reference, string activationCode);
    }
}