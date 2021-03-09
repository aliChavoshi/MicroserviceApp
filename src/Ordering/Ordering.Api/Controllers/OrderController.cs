using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;
using Ordering.Core.Entities;

namespace Ordering.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersByUsername(string userName)
        {
            var query = new GetOrderByUserNameQuery(userName);
            var orders = await _mediator.Send(request: query);
            return Ok(orders);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            var result = await _mediator.Send(request: command);
            return Ok(result);
        }

    }
}
