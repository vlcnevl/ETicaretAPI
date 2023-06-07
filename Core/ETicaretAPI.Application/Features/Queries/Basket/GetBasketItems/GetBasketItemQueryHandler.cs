using ETicaretAPI.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Basket.GetBasketItems
{
    public class GetBasketItemQueryHandler : IRequestHandler<GetBasketItemQueryRequest,List<GetBasketItemQueryResponse>>
    {
        readonly IBasketService _basketService;

        public GetBasketItemQueryHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<List<GetBasketItemQueryResponse>> Handle(GetBasketItemQueryRequest request, CancellationToken cancellationToken)
        {
           var basketItems = await _basketService.GetBasketItemsAsync();

            return basketItems.Select(b => new GetBasketItemQueryResponse
            {
                BasketItemId = b.Id.ToString(),
                Name = b.Product.Name,
                Price = b.Product.Price,
                Quantity = b.Quantity
            }).ToList();
        }
    }
}
