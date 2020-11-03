namespace LoadTestingWebService.Data
{
    public interface IDataStore
    {
        int SetupDataForCompleteProvision(string customerId, string mobileId, string mobileOrderId,
            string phoneNumber, string contactName);

        int SetupDataForActivate(string customerId, string mobileId, string phoneNumber, string activationCode);

        int SetupDataForCompleteActivate(string customerId, string mobileId, string mobileOrderId,
            string phoneNumber);
    }
}