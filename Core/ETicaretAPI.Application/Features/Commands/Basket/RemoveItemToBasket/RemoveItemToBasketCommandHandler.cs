using ETicaretAPI.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Basket.RemoveItemToBasket
{
    public class RemoveItemToBasketCommandHandler : IRequestHandler<RemoveItemToBasketCommandRequest, RemoveItemToBasketCommandResponse>
    {
        readonly IBasketService _basketService;

        public RemoveItemToBasketCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<RemoveItemToBasketCommandResponse> Handle(RemoveItemToBasketCommandRequest request, CancellationToken cancellationToken)
        {
            await _basketService.RemoveItemToBasketAsync(request.BasketItemId);
            return new();
        }
    }
}
