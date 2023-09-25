using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.DTOs.Product;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services
{
    public interface IProductService
    {
        public Task CreateProductAsync(CreateProduct Product);
        public Task RemoveProductAsync(string Id);
        public Task UpdateProductAsync(UpdateProduct Product);
        public Task<ListProduct> GetAllProductsAsync(int page, int size);
        public Task<Product> GetByIdProduct(string Id);
        public Task<byte[]> QRCodeToProduct(string productId);
        public Task UpdateStockToProductAsync(string productId,int stock);
    }
}
