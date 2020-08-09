using System;

namespace ExternalMobileTelecomsNetwork.Api.Data
{
    public class ActivationCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}