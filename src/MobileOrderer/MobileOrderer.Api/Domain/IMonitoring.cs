namespace MobileOrderer.Api.Domain
{
    public interface IMonitoring
    {
        void Activate();
        void ActivateCompleted();
        void Cease();
        void CeaseCompleted();
        void Provision();
        void ProvisionCompleted();
        void CreateCustomer();
    }
}