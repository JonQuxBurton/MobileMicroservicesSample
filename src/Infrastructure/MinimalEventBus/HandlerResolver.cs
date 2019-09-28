using MinimalEventBus.JustSaying;

namespace MinimalEventBus
{
    public class HandlerResolver
    {
        private object rawHandlerWrapper;

        public void Add<T>(HandlerWrapper<T> handlerWrapper) where T : Message
        {
            this.rawHandlerWrapper = handlerWrapper;
        }

        public IHandlerAsync<T> Resolve<T>() where T : Message
        {
            if (rawHandlerWrapper is HandlerWrapper<T>)
            {
                var handlerWrapper = (HandlerWrapper<T>)rawHandlerWrapper;
                return handlerWrapper.GetHandler();
            }

            return null;
        }
    }
}
