using System.Text;
using Bookery.Node.Services.Interfaces;
using RabbitMQ.Client;

namespace Bookery.Node.Services.Implementations
{
    public class StorageProducer : IStorageProducer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public StorageProducer(string hostname, int port, string username, string password)
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostname,
                Port = port,
                UserName = username,
                Password = password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "delete_file",
                durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        public void Delete(Guid id)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            var payload = Encoding.UTF8.GetBytes(id.ToString());

            _channel.BasicPublish(exchange: "", 
                routingKey: "delete_file",
                basicProperties: properties,
                body: payload);
        }

        public void Dispose()
        {
            _connection.Dispose();
            _channel.Dispose();
        }
    }
}
