using MobileTelecomsNetwork.EventHandlers.Data;
using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public interface IOrderCompletedChecker
    {
        Task Check(ActivationOrder sentOrder);
    }
}