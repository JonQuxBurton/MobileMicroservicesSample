using System;

namespace LoadTestingWebService
{
    public class User
    {
        public int Index { get; set; }
        public int VirtualUserId { get; set; }
        public Guid GlobalId { get; set; }
    }
}