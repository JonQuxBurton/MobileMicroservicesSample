using System.Threading.Tasks;

namespace SimCards.EventHandlers.Domain
{
    public interface ISimCardWholesaleService
    {
        Task<bool> PostOrder(SimCardWholesalerOrder order);
    }
}