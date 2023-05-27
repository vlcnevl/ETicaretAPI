using ETicaretAPI.Application.Repositories.ProductRepositories;
using MediatR;
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

        public GetAllProductQueryHandler(IProductReadRepository productReadRepository, ILogger<GetAllProductQueryHandler> logger = null)
        {
            _productReadRepository = productReadRepository;
            _logger = logger;
        }

        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();

            var products = _productReadRepository.GetAll(false).Select(p => new  // anonim tip üretip göndermek istediklerimizi gönderdik
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock,
                p.CreatedDate,
                p.UpdatedDate
            }).Skip(request.Size * request.Page).Take(request.Size).ToList();
            _logger.LogInformation("ürünler listelendi");
            return new() { Products = products, TotalCount = totalCount };

        }
    }
}
