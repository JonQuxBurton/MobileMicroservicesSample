using System;
using System.Collections.Generic;

namespace LoadTestingWebService
{
    public class UsersData
    {
        public Guid UserId { get; set; }
        public Dictionary<string, string>[] Data { get; set; }
    }
}