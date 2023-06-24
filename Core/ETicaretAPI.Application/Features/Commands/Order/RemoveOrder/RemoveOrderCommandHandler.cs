using ETicaretAPI.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.RemoveOrder
{
    public class RemoveOrderCommandHandler : IRequestHandler<RemoveOrderCommandRequest, RemoveOrderCommandResponse>
    {
        readonly IOrderService _orderService;

        public RemoveOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<RemoveOrderCommandResponse> Handle(RemoveOrderCommandRequest request, CancellationToken cancellationToken)
        {
            await _orderService.RemoveOrderAsync(request.OrderId);
            return new();
        }
    }
}
