using System.Threading.Tasks;

namespace SimCards.EventHandlers.Domain
{
    public interface IExternalSimCardsProviderService
    {
        Task<bool> PostOrder(ExternalSimCardOrder order);
    }
}