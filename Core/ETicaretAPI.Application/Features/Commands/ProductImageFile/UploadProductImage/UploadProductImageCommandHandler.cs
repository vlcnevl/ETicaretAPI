using ETicaretAPI.Application.Abstraction.Stroage;
using ETicaretAPI.Application.Repositories.ProductIamgeFileRepositories;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage
{
    public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly IStroageService _stroageService;

        public UploadProductImageCommandHandler(IProductReadRepository productReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IStroageService stroageService)
        {
            _productReadRepository = productReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _stroageService = stroageService;
        }

        public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            //gelecek veriler opsiyonel olduğu için query stringde göndrdik.başka verilerde kullanılabilir.
            List<(string fileName, string pathOrContainerName)> result = await _stroageService.UploadAsync("product-images", request.Files);

            Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.Id);

            await _productImageFileWriteRepository.AddRangeAsync(result.Select(p => new Domain.Entities.ProductImageFile
            {
                FileName = p.fileName,
                Path = p.pathOrContainerName,
                Stroage = _stroageService.StroageName,
                Products = new List<Domain.Entities.Product> { product }

            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();
            return new();
        }
    }
}
