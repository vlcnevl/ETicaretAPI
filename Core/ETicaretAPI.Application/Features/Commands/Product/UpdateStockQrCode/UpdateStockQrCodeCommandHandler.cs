using ETicaretAPI.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateStockQrCode
{
    public class UpdateStockQrCodeCommandHandler : IRequestHandler<UpdateStockQrCodeCommandRequest, UpdateStockQrCodeCommandResponse>
    {
        readonly IProductService _productService;

        public UpdateStockQrCodeCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<UpdateStockQrCodeCommandResponse> Handle(UpdateStockQrCodeCommandRequest request, CancellationToken cancellationToken)
        {
            await _productService.UpdateStockToProductAsync(request.ProductId, request.Stock);
            return new() { };
        }
    }
}
