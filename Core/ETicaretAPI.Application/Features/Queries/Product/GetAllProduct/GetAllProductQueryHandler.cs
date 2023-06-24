using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Product.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository; // applicationdaki bu interface e karsılık concrete nesnesini ıocden getir 
        readonly ILogger<GetAllProductQueryHandler> _logger;
        readonly IProductService _productService;
        public GetAllProductQueryHandler(IProductReadRepository productReadRepository, ILogger<GetAllProductQueryHandler> logger = null, IProductService productService = null)
        {
            _productReadRepository = productReadRepository;
            _logger = logger;
            _productService = productService;
        }

        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            //var totalCount = _productReadRepository.GetAll(false).Count();

            //var products = _productReadRepository.GetAll(false).Include(p => p.ProductImageFiles).Select(p => new  // anonim tip üretip göndermek istediklerimizi gönderdik
            //{
            //    p.Id,
            //    p.Name,
            //    p.Description,
            //    p.Price,
            //    p.Stock,
            //    p.CreatedDate,
            //    p.UpdatedDate,
            //    p.ProductImageFiles
            //}).Skip(request.Size * request.Page).Take(request.Size).ToList();

           _logger.LogInformation("ürünler listelendi");

            var products = await _productService.GetAllProductsAsync(request.Page, request.Size);

            return new() { Products = products.Products, TotalCount = products.TotalCount };

           

        }
    }
}
