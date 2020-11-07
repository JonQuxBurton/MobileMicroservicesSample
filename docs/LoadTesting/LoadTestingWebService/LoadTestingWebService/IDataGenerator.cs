using System;

namespace LoadTestingWebService
{
    public interface IDataGenerator
    {
        string GetExistingCustomerId();
        string GetNextPhoneNumber();
        string GetNextContactPhoneNumber();
        string GetNextContactName();
        string GetNextActivationCode();
        Guid GetNextGuid();
    }
}