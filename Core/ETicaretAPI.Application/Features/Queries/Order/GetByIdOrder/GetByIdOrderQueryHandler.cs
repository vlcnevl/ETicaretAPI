using ETicaretAPI.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Order.GetByIdOrder
{
    public class GetByIdOrderQueryHandler : IRequestHandler<GetByIdOrderQueryRequest, GetByIdOrderQueryResponse>
    {
        readonly IOrderService _orderService;

        public GetByIdOrderQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetByIdOrderQueryResponse> Handle(GetByIdOrderQueryRequest request, CancellationToken cancellationToken)
        {
            var data = await _orderService.GetByIdOrderAsync(request.Id);

            return new()
            {
                Id = data.Id,
                Description = data.Description,
                Address = data.Address,
                BasketItems = data.BasketItems,
                CreatedDate = data.CreatedDate,
                OrderCode = data.OrderCode,
            };
        }
    }
}
