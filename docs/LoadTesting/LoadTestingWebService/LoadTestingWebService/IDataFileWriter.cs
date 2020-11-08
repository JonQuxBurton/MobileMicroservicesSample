using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public interface IDataFileWriter
    {
        void WriteDataFile(Dictionary<string, List<UsersData>> allData);
    }
}