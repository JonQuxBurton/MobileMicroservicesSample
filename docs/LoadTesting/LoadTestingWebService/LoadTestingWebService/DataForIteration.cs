using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class DataForIteration
    {
        public DataForIteration(Guid userId, Dictionary<string, string>[] data)
        {
            UserId = userId;
            Data = data;
        }

        public Guid UserId { get; }
        public Dictionary<string, string>[] Data { get; }
    }
}