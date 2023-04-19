using RabbitMQ.Client;

namespace Bunnymail.Messaging
{
    public interface IMessenger
    {
        /// <summary>
        /// Declares and registers events for consumers
        /// </summary>
        /// <param name="channel"></param>
        void Register(IModel channel);
    }
}
