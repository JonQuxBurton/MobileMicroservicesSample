using System;
using System.Threading.Tasks;
using MinimalEventBus.JustSaying;
using MinimalEventBus.Tests;

namespace SimCards.EventHandlers
{
    public class TestHandler : IHandlerAsync<TestMessage>
    {
        public Task<bool> Handle(TestMessage message)
        {
            return Task.FromResult(true);
        }

    }
}
