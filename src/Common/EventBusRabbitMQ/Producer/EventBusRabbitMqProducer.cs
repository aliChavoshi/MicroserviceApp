using EventBusRabbitMQ.Event;
using Newtonsoft.Json;
using System;
using System.Text;

namespace EventBusRabbitMQ.Producer
{
    public class EventBusRabbitMqProducer
    {
        private readonly IRabbitMqConnection _connection;
        public EventBusRabbitMqProducer(IRabbitMqConnection connection)
        {
            _connection = connection;
        }
        public void PublishBasketCheckout(string queueName, BasketCheckoutEvent publishModel)
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            var message = JsonConvert.SerializeObject(publishModel);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;

            channel.ConfirmSelect();
            channel.BasicPublish(exchange: "", routingKey: queueName,
                mandatory: true, basicProperties: properties, body: body);
            channel.WaitForConfirmsOrDie();

            channel.BasicAcks += (sender, eventArgs) =>
            {
                Console.WriteLine("Send RabbitMQ");
            };
            channel.ConfirmSelect();
        }
    }
}