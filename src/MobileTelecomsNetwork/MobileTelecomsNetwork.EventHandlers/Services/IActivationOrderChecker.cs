using MobileTelecomsNetwork.EventHandlers.Data;
using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Services
{
    public interface IActivationOrderChecker
    {
        Task Check(ActivationOrder sentOrder);
    }
}