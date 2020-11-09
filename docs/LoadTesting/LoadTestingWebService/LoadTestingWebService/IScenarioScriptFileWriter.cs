using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IScenarioScriptFileWriter
    {
        void Write(List<DataForScenario> dataInFormatForFile);
    }
}