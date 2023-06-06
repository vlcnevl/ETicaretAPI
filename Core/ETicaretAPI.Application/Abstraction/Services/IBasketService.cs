using ETicaretAPI.Application.ViewModels.Basket;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services
{
    public interface IBasketService
    {
        public Task<List<BasketItem>> GetBasketItemAsync();
        public Task AddItemToBasketAsync(CreateBasketItemViewModel basketItem);
        public Task UpdateQuantityAsync(UpdateBasketItemViewModel basketItem);
        public Task RemoveItemToBasketAsync(string basketItemId);
    }
}
