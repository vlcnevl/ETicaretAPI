﻿using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.DTOs.Product;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistance.Services
{
    public class ProductService : IProductService
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;

        public ProductService(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        public async Task CreateProductAsync(CreateProduct Product)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Name = Product.Name,
                Price = Product.Price,
                Stock = Product.Stock,
                Description = Product.Description

            });
            await _productWriteRepository.SaveAsync();
        }

        public async Task<ListProduct> GetAllProductsAsync(int page, int size)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();

            var products = _productReadRepository.GetAll(false).Include(p => p.ProductImageFiles).Select(p => new  // anonim tip üretip göndermek istediklerimizi gönderdik
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock,
                p.CreatedDate,
                p.UpdatedDate,
                p.ProductImageFiles
            }).Skip(size * page).Take(size).ToList();


            return new() { Products = products, TotalCount = totalCount };



        }

        public async Task<Product> GetByIdProduct(string Id)
        {
            Product product = await _productReadRepository.GetByIdAsync(Id);
            return product;
        }

        public async Task RemoveProductAsync(string Id)
        {
           await _productWriteRepository.RemoveAsync(Id);
           await _productWriteRepository.SaveAsync();
        }

        public async Task UpdateProductAsync(UpdateProduct product)
        {
            Product updateProduct = await _productReadRepository.GetByIdAsync(product.Id.ToString());

            updateProduct.Stock = product.Stock;
            updateProduct.Description = product.Description;
            updateProduct.Name = product.Name;
            updateProduct.Price = product.Price;

            await _productWriteRepository.SaveAsync();
        }
    }
}
