using System.Threading.Tasks;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public interface IOrderCompletedChecker
    {
        Task Check(Order sentOrder);
    }
}