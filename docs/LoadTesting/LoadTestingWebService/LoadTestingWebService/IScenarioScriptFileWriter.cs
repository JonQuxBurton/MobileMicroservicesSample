using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IScenarioScriptFileWriter
    {
        void Write(Dictionary<string, Dictionary<Guid, Dictionary<string, string>[]>>  dataInFormatForFile);
    }
}