using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IScenarioScriptFileWriter
    {
        void Write(Dictionary<string, List<UsersData>>  dataInFormatForFile);
    }
}