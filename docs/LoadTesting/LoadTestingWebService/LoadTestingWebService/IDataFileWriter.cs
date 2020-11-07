using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IDataFileWriter
    {
        void WriteDataFile(Dictionary<string, Dictionary<Guid, Dictionary<string, string>[]>> allData);
    }
}