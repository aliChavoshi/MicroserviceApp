using System;
using AutoMapper;
using Basket.Api.Entities;
using EventBusRabbitMQ.Event;

namespace Basket.Api.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BasketCheckout, BasketCheckoutEvent>()
                .ForMember(x => x.RequestId,
                    c =>
                        c.MapFrom(v => Guid.NewGuid()));
        }
    }
}