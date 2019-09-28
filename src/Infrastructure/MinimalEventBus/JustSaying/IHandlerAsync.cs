using System.Threading.Tasks;

namespace MinimalEventBus.JustSaying
{
    public interface IHandlerAsync<in T>
    {
        /// <summary>
        /// Handle a message of a given type
        /// </summary>
        /// <param name="message">Message to handle</param>
        /// <returns>Was handling successful?</returns>
        Task<bool> Handle(T message);
    }
}
