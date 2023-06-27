using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services
{
    public interface IOrderService
    {
        public Task CreateOrderAsync(CreateOrder createOrder);
        public Task<ListOrder> GetAllOrdersAsync(int page,int size);
        public Task<SingleOrder> GetByIdOrderAsync(string id);
        public Task RemoveOrderAsync(string orderId);
    }
}
