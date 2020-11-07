using System.Collections.Generic;

namespace LoadTestingWebService.Data
{
    public interface IDataStore
    {
        int SetupData(string scenario, Dictionary<string, string> data);
    }
}