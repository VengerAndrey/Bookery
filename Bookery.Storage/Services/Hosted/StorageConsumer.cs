using System.Text;
using Bookery.Storage.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Bookery.Storage.Services.Hosted
{
    public class StorageConsumer : IHostedService, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IModel? _channel;
        private EventingBasicConsumer? _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public StorageConsumer(IServiceScopeFactory serviceScopeFactory, string hostname, int port, string username,
            string password)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = hostname,
                Port = port,
                UserName = username,
                Password = password
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var retries = 0;

            while (retries < 3)
            {
                retries++;
                try
                {
                    _connection = _connectionFactory.CreateConnection();
                    break;
                }
                catch (BrokerUnreachableException)
                {
                    await Task.Delay((int)Math.Pow(2, retries) * 1000, cancellationToken);
                }
            }

            _channel = _connection?.CreateModel();
            _channel?.QueueDeclare(queue: "delete_file",
                durable: false, exclusive: false, autoDelete: false, arguments: null);

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (sender, args) =>
            {
                var id = Guid.Parse(Encoding.UTF8.GetString(args.Body.Span));
                await DeleteFile(id);
            };
            _channel?.BasicConsume(queue: "delete_file", autoAck: true, consumer: _consumer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }

        private async Task DeleteFile(Guid id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();
            await storageService.Delete(id);
        }
    }
}