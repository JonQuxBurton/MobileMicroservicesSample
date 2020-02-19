using SimCards.EventHandlers.Data;
using System.Threading.Tasks;

namespace SimCards.EventHandlers
{
    public interface ICompletedOrderChecker
    {
        Task Check(SimCardOrder sentOrder);
    }
}