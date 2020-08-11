using System;

namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class ExternalOrder
    {
        public int Id { get; set; }
        public Guid Reference { get; set; }
        public string Name { get; set; }
        public string Status
        {
            get { return status; }
            set { status = value.Trim(); }
        }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        private string status;
    }
}
