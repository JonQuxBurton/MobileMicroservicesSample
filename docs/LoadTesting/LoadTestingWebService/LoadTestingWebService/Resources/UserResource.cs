using System;

namespace LoadTestingWebService.Resources
{
    public class UserResource
    {
        public int Index { get; set; }
        public int VirtualUserId { get; set; }
        public Guid GlobalId { get; set; }
    }
}