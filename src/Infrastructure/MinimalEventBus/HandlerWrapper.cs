using MinimalEventBus.JustSaying;

namespace MinimalEventBus
{
    public class HandlerWrapper<T> where T : Message
    {
        private readonly IHandlerAsync<T> handler;

        public HandlerWrapper(IHandlerAsync<T> handler)
        {
            this.handler = handler;
        }

        public IHandlerAsync<T> GetHandler()
        {
            return handler;
        }
    }
}
