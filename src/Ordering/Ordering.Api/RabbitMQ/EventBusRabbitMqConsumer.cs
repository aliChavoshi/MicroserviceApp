﻿using AutoMapper;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Event;
using MediatR;
using Newtonsoft.Json;
using Ordering.Application.Commands;
using Ordering.Core.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Api.RabbitMQ
{
    public class EventBusRabbitMqConsumer
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public EventBusRabbitMqConsumer(IRabbitMqConnection connection, IMediator mediator,
            IMapper mapper, IOrderRepository orderRepository)
        {
            _connection = connection;
            _mediator = mediator;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public void Consume()
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.BasketCheckoutQueue,
                durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(model: channel);
            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: EventBusConstants.BasketCheckoutQueue,
                autoAck: true, consumer: consumer, noLocal: false, exclusive: false, arguments: null);
        }

        private async Task ReceivedEvent(object sender, BasicDeliverEventArgs args)
        {
            if (args.RoutingKey == EventBusConstants.BasketCheckoutQueue)
            {
                var message = Encoding.UTF8.GetString(args.Body.Span);
                var basketCheckoutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);

                var command = _mapper.Map<CheckoutOrderCommand>(basketCheckoutEvent);
                var result = await _mediator.Send(command);
            }
        }

        public void Disconnect()
        {
            _connection.Dispose();
        }
    }
}