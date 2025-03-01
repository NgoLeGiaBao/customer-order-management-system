using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace WelcomeService.RabbitMQ
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly string _hostName = "localhost";  

        public void Publish<T>(string queue, T message)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Ensure the queue exists before we publish to it
            channel.QueueDeclare(queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            // Send message to queue
            channel.BasicPublish(
                exchange: "",
                routingKey: queue,
                basicProperties: null,
                body: body
            );

        }
    }
}
