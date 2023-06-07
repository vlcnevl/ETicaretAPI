using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.Repositories.BasketItemRepositories;
using ETicaretAPI.Application.Repositories.BasketRepositories;
using ETicaretAPI.Application.Repositories.OrderRepositories;
using ETicaretAPI.Application.ViewModels.Basket;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    //authentice olmus kullanıcının bir sepeti olcak.sepet ordera baglandıysa islemi tamam yeni sepet yoksa o sepetle islem.
    //buradaki kurdugumuz mantık ordera bağlanmamıs basketi kullanmak.eğer yoksa olustur.varsa getir ona islem yap.
    public class BasketService : IBasketService
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly UserManager<AppUser> _userManager;
        readonly IOrderReadRepository _orderReadRepository; 
        readonly IBasketWriteRepository _basketWriteRepository;
        readonly IBasketReadRepository _basketReadRepository;
        readonly IBasketItemWriteRepository _basketItemWriteRepository;
        readonly IBasketItemReadRepository _basketItemReadRepository;

        public BasketService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IOrderReadRepository orderReadRepository, IBasketWriteRepository basketWriteRepository, IBasketItemWriteRepository basketItemWriteRepository, IBasketItemReadRepository basketItemReadRepository = null, IBasketReadRepository basketReadRepository = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _orderReadRepository = orderReadRepository;
            _basketWriteRepository = basketWriteRepository;
            _basketItemWriteRepository = basketItemWriteRepository;
            _basketItemReadRepository = basketItemReadRepository;
            _basketReadRepository = basketReadRepository;
        }

        private async Task<Basket?> GetUserBasket()
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(username))
            {
                //gelen username ile ilgili basketleri getir.
                AppUser? user = await _userManager.Users.Include(u => u.Baskets).FirstOrDefaultAsync(u => u.UserName == username);

                var _basket = from basket in user.Baskets
                              join order in _orderReadRepository.Table
                              on basket.Id equals order.Id into BasketOrders
                              from order in BasketOrders.DefaultIfEmpty()
                              select new
                              {
                                  Basket = basket,
                                  Order = order
                              };

                Basket? targetBasket = null;
                if (_basket.Any(b => b.Order is null))
                    targetBasket = _basket.FirstOrDefault(b => b.Order is null)?.Basket;
                else
                {
                    targetBasket = new();
                    user.Baskets.Add(targetBasket);
                }
                    


                await _basketWriteRepository.SaveAsync();
                return targetBasket;
            }
            else
                throw new Exception("Kullanıcı bulunamadı");
        

        }


        public async Task AddItemToBasketAsync(CreateBasketItemViewModel basketItem)
        {
            Basket? basket = await GetUserBasket();
            
            if(basket != null)
            {
               BasketItem _basketItem = await _basketItemReadRepository.GetSingleAsync(bi=> bi.BasketId == basket.Id && bi.ProductId == Guid.Parse(basketItem.ProductId));

                if (_basketItem != null)
                    _basketItem.Quantity++;
                else
                   await _basketItemWriteRepository.AddAsync(new()
                    {
                        BasketId = basket.Id,
                        ProductId = Guid.Parse(basketItem.ProductId),
                        Quantity = basketItem.Quantity,
                    });
            
                await _basketWriteRepository.SaveAsync();
            }
        }

        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            Basket? basket = await GetUserBasket();
            //
            Basket? basketResult= await  _basketReadRepository.Table.Include(b=> b.BasketItems).ThenInclude(bi=> bi.Product).FirstOrDefaultAsync(b=> b.Id == basket.Id);
            return basketResult.BasketItems.ToList();
        }

        public async Task RemoveItemToBasketAsync(string basketItemId)
        {
            BasketItem? basketItem = await _basketItemReadRepository.GetByIdAsync(basketItemId);
            if (basketItem != null)
            {
                _basketItemWriteRepository.Remove(basketItem);
               await _basketItemWriteRepository.SaveAsync();
            }

        }

        public async Task UpdateQuantityAsync(UpdateBasketItemViewModel basketItem)
        {
            BasketItem? _basketItem = await _basketItemReadRepository.GetByIdAsync(basketItem.BasketItemId);

            if (_basketItem != null)
            {
                _basketItem.Quantity = basketItem.Quantity;
               await _basketItemWriteRepository.SaveAsync();
            }
        }
    }
}
