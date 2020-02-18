using MobileOrderer.Api.Domain;

namespace MobileOrderer.Api.Services
{
    public interface IMobileCommand
    {
        void Execute(Mobile mobile);
    }
}