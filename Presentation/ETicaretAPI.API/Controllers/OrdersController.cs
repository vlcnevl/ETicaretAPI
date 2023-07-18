using ETicaretAPI.Application.Features.Commands.Order.CompleteOrder;
using ETicaretAPI.Application.Features.Commands.Order.CreateOrder;
using ETicaretAPI.Application.Features.Commands.Order.RemoveOrder;
using ETicaretAPI.Application.Features.Queries.Order.GetAllOrder;
using ETicaretAPI.Application.Features.Queries.Order.GetByIdOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]

    public class OrdersController : ControllerBase
    {
        readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost] 
        public async Task<IActionResult> CreateOrder(CreateOrderCommandRequest request)
        {
            CreateOrderCommandResponse response = await _mediator.Send(request);
            return Ok(response);    
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetAllOrdersQueryRequest request)
        {
           GetAllOrdersQueryResponse response = await _mediator.Send(request);  
            return Ok(response);
        }

        [HttpDelete("{OrderId}")]
        public async Task<IActionResult> Remove([FromRoute] RemoveOrderCommandRequest request)
        {
            RemoveOrderCommandResponse response =  await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] GetByIdOrderQueryRequest request)
        {
            GetByIdOrderQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("complete-order/{id}")]
        public async Task<IActionResult> CompleteOredr([FromRoute]CompleteOrderCommandRequest request)
        {
            CompleteOrderCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }            
    }
}
