using System.Threading;
using System.Threading.Tasks;

namespace MobileOrderer.Api.Services
{
    public interface IMobileEventCheckersRunner
    {
        void Check();
        Task StartChecking(CancellationToken stoppingToken);
    }
}