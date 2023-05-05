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
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _configuration;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IStroageService stroageService, IProductImageFileWriteRepository productImageFileWriteRepository, IConfiguration configuration)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _stroageService = stroageService;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _configuration = configuration;
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
        public async Task<IActionResult> Upload(string id) // hangi ürüne dosya yüklendiğini id'den ayırt edeceğiz.QueryString'ten geliyo
        {//gelecek veriler opsiyonel olduğu için query stringde göndrdik.başka verilerde kullanılabilir.
          List<(string fileName,string pathOrContainerName)> result = await _stroageService.UploadAsync("product-images",Request.Form.Files);

          Product product = await _productReadRepository.GetByIdAsync(id);

            //foreach (var file in result)
            //{
            //    product.ProductImageFiles.Add(new()
            //    {
            //        FileName = file.fileName,
            //        Path = file.pathOrContainerName,
            //        Stroage = _stroageService.StroageName,
            //    });
            //}
            


            await _productImageFileWriteRepository.AddRangeAsync(result.Select(p => new ProductImageFile()
            {
                FileName = p.fileName,
                Path = p.pathOrContainerName,
                Stroage = _stroageService.StroageName,
                Products = new List<Product> { product }

            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpGet("[action]/{id}")] //route'dan geliyor
        public async Task<IActionResult> GetProductImages(string id)
        {
         Product? product = await _productReadRepository.Table.Include(p=> p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));  // imageleri ile productu çektik
           
            return Ok(product?.ProductImageFiles.Select(p=> new 
            {
             Path = $"{_configuration["BaseStroageUrl"]}/{p.Path}",
             p.FileName,
             p.Id
            }));
        }

        [HttpDelete("[action]/{productId}/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(string productId,string imageId)
        {
          Product? product =   await _productReadRepository.Table.Include(p=> p.ProductImageFiles).FirstOrDefaultAsync(p=> p.Id == Guid.Parse(productId));

           ProductImageFile productImageFile = product.ProductImageFiles.FirstOrDefault(i => i.Id == Guid.Parse(imageId));
            
            product.ProductImageFiles.Remove(productImageFile);
           await _productWriteRepository.SaveAsync();
            return Ok();
        }


    }
}
