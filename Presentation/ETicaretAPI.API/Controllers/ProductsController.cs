using ETicaretAPI.Application.Abstraction.Stroage;
using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct;
using ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImage;
using ETicaretAPI.Application.Repositories.ProductIamgeFileRepositories;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes ="Admin")]
    public class ProductsController : ControllerBase
    {
        readonly IMediator _mediator;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IStroageService stroageService, IProductImageFileWriteRepository productImageFileWriteRepository, IConfiguration configuration, IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest request)
        {
            GetAllProductQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest request)
        {
            GetByIdProductQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateProductCommandRequest request)
        {
            await _mediator.Send(request);
            return StatusCode((int)HttpStatusCode.Created);

        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest request)
        {
            UpdateProductCommandResponse response = await _mediator.Send(request);
            return Ok();
        }


        [HttpDelete("{Id}")] // request model içerisindeki Id ile bind olabilmesi için
        public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest request)
        {
            RemoveProductCommandResponse response = await _mediator.Send(request);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest request) // hangi ürüne dosya yüklendiğini id'den ayırt edeceğiz.QueryString'ten geliyo
        {
            request.Files = Request.Form.Files;
            UploadProductImageCommandResponse response = await _mediator.Send(request);
            return Ok();
        }

        [HttpGet("[action]/{Id}")] //route'dan geliyor
        public async Task<IActionResult> GetProductImages([FromRoute] GetProductImagesQueryRequest request)
        {
          List<GetProductImagesQueryResponse> response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("[action]/{Id}")] // iki değer normal parametre olarak karşılanıp içerde instance a bind edilebilir.
        public async Task<IActionResult> DeleteProductImage([FromRoute] RemoveProductImageCommandRequest request, [FromQuery] string imageId)
        {
            request.ImageId = imageId;
           RemoveProductImageCommandResponse response = await _mediator.Send(request);
            return Ok();
        }


    }
}
