using System.Threading.Tasks;

namespace SimCards.EventHandlers.Services
{
    public interface ISimCardWholesaleService
    {
        Task<bool> PostOrder(SimCardWholesalerOrder order);
    }
}