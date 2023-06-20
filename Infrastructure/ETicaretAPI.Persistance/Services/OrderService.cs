using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories.OrderRepositories;
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

        public OrderService(IOrderWriteRepository orderWriteRepository)
        {
            _orderWriteRepository = orderWriteRepository;
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
          await _orderWriteRepository.AddAsync(new() { Address = createOrder.Address , Description = createOrder.Description, Id =Guid.Parse(createOrder.BasketId)});
            await _orderWriteRepository.SaveAsync();
        }
    }
}
