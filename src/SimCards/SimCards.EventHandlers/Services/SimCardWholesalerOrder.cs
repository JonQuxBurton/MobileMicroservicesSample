using System;
using SimCards.EventHandlers.Data;

namespace SimCards.EventHandlers.Services
{
    public class SimCardWholesalerOrder
    {
        public Guid Reference { get; set; }
        public string Name { get; set; }
    }
}