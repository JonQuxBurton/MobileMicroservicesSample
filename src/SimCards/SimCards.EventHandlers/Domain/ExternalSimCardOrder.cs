using System;

namespace SimCards.EventHandlers.Domain
{
    public class ExternalSimCardOrder
    {
        public Guid Reference { get; set; }
        public string Name { get; set; }
    }
}