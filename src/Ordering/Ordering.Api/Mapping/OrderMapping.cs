using AutoMapper;
using EventBusRabbitMQ.Event;
using Ordering.Application.Commands;

namespace Ordering.Api.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<BasketCheckoutEvent, CheckoutOrderCommand>().ReverseMap();
        }
    }
}