using System;
using Basket.Api.Entities;
using Basket.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Event;
using EventBusRabbitMQ.Producer;

namespace Basket.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]/[action]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly EventBusRabbitMqProducer _eventBus;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, EventBusRabbitMqProducer eventBus, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _eventBus = eventBus;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBasket(string userName)
        {
            return Ok(await _basketRepository.GetBasket(userName) ?? new BasketCart() { UserName = userName });
        }

        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBasket([FromBody] BasketCart basketCart)
        {
            return Ok(await _basketRepository.UpdateBasket(basketCart));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            return Ok(await _basketRepository.DeleteBasket(userName));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)] //202
        [ProducesResponseType((int)HttpStatusCode.BadRequest)] //400
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //get total price of basket
            //remove the basket
            //send checkout event to rabbitMQ

            var basket = await _basketRepository.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BadRequest();

            var basketRemoved = await _basketRepository.DeleteBasket(basket.UserName);
            if (!basketRemoved)
                return BadRequest();

            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice();

            _eventBus.PublishBasketCheckout(EventBusConstants.BasketCheckoutQueue, eventMessage);

            return Accepted();
        }
    }
}
