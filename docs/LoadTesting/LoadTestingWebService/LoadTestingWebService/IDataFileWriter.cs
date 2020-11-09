using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IDataFileWriter
    {
        void WriteDataFile(List<DataForScenario> allData);
    }
}