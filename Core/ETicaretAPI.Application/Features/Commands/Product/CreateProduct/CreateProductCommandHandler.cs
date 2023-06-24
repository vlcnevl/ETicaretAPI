using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.Abstraction.Services.Hubs;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommandRequest, CreateProductCommandResponse>
    {
        //readonly IProductWriteRepository _productWriteRepository;
        readonly IProductHubService _productHubService;
        readonly IProductService _productService;
        public CreateProductCommandHandler(/*IProductWriteRepository productWriteRepository,*/ IProductHubService productHubService = null, IProductService productService = null)
        {
            //_productWriteRepository = productWriteRepository;
            _productHubService = productHubService;
            _productService = productService;
        }

        public async Task<CreateProductCommandResponse> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
        {
            //await _productWriteRepository.AddAsync(new()
            //{
            //    Name = request.Name,
            //    Price = request.Price,
            //    Stock = request.Stock,
            //    Description = request.Description

            //});
            //await _productWriteRepository.SaveAsync();
            await _productService.CreateProductAsync(new() {Description = request.Description,Name = request.Name, Price = request.Price, Stock = request.Stock });

            await _productHubService.ProductAddedMessageAsync($"{request.Name} isminde ürün eklenmistir."); //signalr

            return new();
        }
    }
}
