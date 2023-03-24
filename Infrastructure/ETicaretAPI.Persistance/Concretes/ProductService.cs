using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Concretes
{
    public class ProductService : IProductService
    {
        public List<Product> GetProducts() => new () 
        { 
            new () { Id = Guid.NewGuid(), Name = "ürün 1" ,Price = 100 ,Stock=10, Description="ürünn1" },   
            new () { Id = Guid.NewGuid(), Name = "ürün 2" ,Price = 200 ,Stock=20, Description="ürünn2" },   
            new () { Id = Guid.NewGuid(), Name = "ürün 3" ,Price = 300 ,Stock=30, Description="ürünn3" },
            new () { Id = Guid.NewGuid(), Name = "ürün 4" ,Price = 400 ,Stock=40, Description="ürünn4" },  
            new () { Id = Guid.NewGuid(), Name = "ürün 5" ,Price = 500 ,Stock=50, Description="ürünn5" }   
            
        };
    }
}
