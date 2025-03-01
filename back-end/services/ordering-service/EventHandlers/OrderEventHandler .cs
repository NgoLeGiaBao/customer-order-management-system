using System.Text;
using System.Text.Json;
using order_service.Data;
using order_service.Enums;
using order_service.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace order_service.EventHandlers
{
    public class OrderEventHandler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderEventHandler> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public OrderEventHandler(IServiceScopeFactory scopeFactory, ILogger<OrderEventHandler> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "table.opened", durable: false, exclusive: false, autoDelete: false, arguments: null);

            _logger.LogInformation("Waiting for events...");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Deserialize the message
                var eventData = JsonSerializer.Deserialize<JsonElement>(message);
                var tableNumber = eventData.GetProperty("TableNumber").GetInt32();
                var employeeId = eventData.GetProperty("EmployeeId").GetInt32();

                await HandleTableOpenedEvent(tableNumber, employeeId);
            };

            _channel.BasicConsume(queue: "table.opened", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task HandleTableOpenedEvent(int tableNumber, int employeeId)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var order = new Order
            {
                TableId = tableNumber,
                EmployeeId = employeeId,
                Status = OrderStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            _logger.LogInformation($"Order created for Table {tableNumber} by Employee {employeeId}");
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
