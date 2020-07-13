using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public interface IExternalMobileTelecomsNetworkService
    {
        Task<bool> PostCancel(ExternalMobileTelecomsNetworkOrder order);
        Task<bool> PostOrder(ExternalMobileTelecomsNetworkOrder order);
    }
}