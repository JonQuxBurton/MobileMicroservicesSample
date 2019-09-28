using MinimalEventBus.JustSaying;
using System.Threading.Tasks;

namespace MinimalEventBus
{
    public interface IHandler
    {
        Task<bool> Handle(in Message message);
    }
}
