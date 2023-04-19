using RabbitMQ.Client;

namespace Bunnymail.Messaging
{
    public class RabbitMQClient
    {
        IConnection connection;
        IModel channel;

        static RabbitMQClient? _instance;
        public static RabbitMQClient Instance => _instance ?? throw new NotImplementedException("RabbitMQClient.Instance has not been prepared.");

        public IModel Channel => channel;

        private readonly IList<IMessenger> _consumers = new List<IMessenger>();

        public RabbitMQClient(IConfiguration config)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["rabbithost"],
                UserName = config["rabbituser"],
                Password = config["rabbitpass"]
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.BasicQos(0, 30, false);

            _instance = this;
        }

        public RabbitMQClient RegisterConsumer(IMessenger consumer)
        {
            consumer.Register(channel);
            _consumers.Add(consumer);
            return this;
        }

        public void Close()
        {
            _consumers.Clear();
            channel.Close();
            channel.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
