using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public interface IExternalMobileTelecomsNetworkService
    {
        Task<bool> PostCease(ExternalMobileTelecomsNetworkOrder order);
        Task<bool> PostOrder(ExternalMobileTelecomsNetworkOrder order);
    }
}