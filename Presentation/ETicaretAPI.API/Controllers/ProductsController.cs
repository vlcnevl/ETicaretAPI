using ETicaretAPI.Application.Abstraction.Stroage;
using ETicaretAPI.Application.Repositories.File;
using ETicaretAPI.Application.Repositories.ProductIamgeFileRepositories;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IStroageService _stroageService;
        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IStroageService stroageService, IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _stroageService = stroageService;
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Pagination pagination)
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
            }).Skip(pagination.Size * pagination.Page).Take(pagination.Size);

            return Ok(new
            {
                products,
                totalCount
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id, false));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {

            }

            await _productWriteRepository.AddAsync(new()
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
                Description = model.Description

            });
            await _productWriteRepository.SaveAsync();
            return StatusCode((int)HttpStatusCode.Created);

        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdateProductViewModel model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);

            product.Stock = model.Stock;
            product.Price = model.Price;
            product.Name = model.Name;
            product.Description = model.Description;

            await _productWriteRepository.SaveAsync();

            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload()
        {
          var datas = await _stroageService.UploadAsync("files",Request.Form.Files);

           await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            {
                FileName = d.fileName,
                Path = d.pathOrContainerName,
                Stroage = _stroageService.StroageName
            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();
            return Ok();
        }


    }
}
