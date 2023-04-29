using ETicaretAPI.Application.Repositories.File;
using ETicaretAPI.Application.Repositories.ProductIamgeFileRepositories;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.Services;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
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
        private readonly IFileService  _fileService;
        private readonly IFileWriteRepository _fileWriteRepository;
        private readonly IFileReadRepository _fileReadRepository;
        private readonly IProductImageFileReadRepository _productImageFileReadRepository;
        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileService fileService, IFileWriteRepository fileWriteRepository, IFileReadRepository fileReadRepository, IProductImageFileReadRepository productImageFileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _fileService = fileService;
            _fileWriteRepository = fileWriteRepository;
            _fileReadRepository = fileReadRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]Pagination pagination)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();

          var products = _productReadRepository.GetAll(false).Select(p=> new  // anonim tip üretip göndermek istediklerimizi gönderdik
            {
               p.Id,
               p.Name,
               p.Description,
               p.Price,
               p.Stock,
               p.CreatedDate,
               p.UpdatedDate
            }).Skip(pagination.Size*pagination.Page).Take(pagination.Size);

            return Ok(new
            {
                 products,
                 totalCount
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id,false));
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
            return StatusCode((int) HttpStatusCode.Created);

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
           var datas = await _fileService.UploadAsync("files",Request.Form.Files);


            //await  _productImageFileWriteRepository.AddRangeAsync(datas.Select(d=> new ProductImageFile() { 
            //  FileName = d.fileName,
            //  Path = d.path,  
            //}).ToList());

            // await _productImageFileWriteRepository.SaveAsync();

            await _fileWriteRepository.AddRangeAsync(datas.Select(d => new Domain.Entities.File()
            {
                FileName = d.fileName,
                Path = d.path,  
            }).ToList()) ;

            await _productImageFileWriteRepository.SaveAsync();
          return Ok();



            // BASİT DOSYA YÜKLEME OPERASYONU WWWROOTA

           // //wwwroot/resource/productImages
           // string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,"resource/productImages");


           // if(!Directory.Exists(uploadPath))
           //     Directory.CreateDirectory(uploadPath);
            
           // Random rand = new();
           // foreach (IFormFile file in Request.Form.Files)
           // {
           //     string fullPath = Path.Combine(uploadPath,$"{rand.Next()}{Path.GetExtension(file.FileName)}");

           //     using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None,1024*1024,useAsync:false);
            
           //     await file.CopyToAsync(fileStream);
           //     await fileStream.FlushAsync();
           // }
           
        }

    }
}
