using SimCards.EventHandlers.Data;
using System.Threading.Tasks;

namespace SimCards.EventHandlers.Domain
{
    public interface ICompletedOrderChecker
    {
        Task Check(SimCardOrder sentOrder);
    }
}