using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.ViewModels.Products
{
    public class CreateProductViewModel
    {
        public string Name { get; set;}
        public int Stock { get; set;}
        public float Price { get; set;}
        public string Description { get; set;}
    }
}
