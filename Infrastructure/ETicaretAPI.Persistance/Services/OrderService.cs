using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories.OrderRepositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
            var orderCode = (new Random().NextDouble()*10000).ToString();

            orderCode = orderCode.Substring(orderCode.IndexOf(",") + 1 , orderCode.Length - orderCode.IndexOf(",") - 1 );

            await _orderWriteRepository.AddAsync(new() { Address = createOrder.Address, Description = createOrder.Description, Id = Guid.Parse(createOrder.BasketId), OrderCode = orderCode });
            await _orderWriteRepository.SaveAsync();
        }

        public async Task<ListOrder> GetAllOrdersAsync(int page, int size)
        {
            var query = _orderReadRepository.Table.Include(o => o.Basket)
                                               .ThenInclude(b => b.User)
                                               .Include(u => u.Basket)
                                                   .ThenInclude(b => b.BasketItems)
                                                   .ThenInclude(p => p.Product);
          var data = query.Skip(page*size).Take(size);

            return new()
            {
                TotalCount = await query.CountAsync(),
                Orders = await data.Select(order => new
                {

                    Id = order.Id,
                    CreatedDate = order.CreatedDate,
                    OrderCode = order.OrderCode,
                    TotalPrice = order.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    UserName = order.Basket.User.UserName,
                    Address = order.Address
                }).ToListAsync()

            };

        }
      




        public async Task RemoveOrderAsync(string orderId)
        {
            Order? order = await _orderReadRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                _orderWriteRepository.Remove(order);
               await _orderWriteRepository.SaveAsync();
            }

        }
    }
}
