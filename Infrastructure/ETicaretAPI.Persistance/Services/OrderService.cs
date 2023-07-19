using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories.CompleteOrderRepositories;
using ETicaretAPI.Application.Repositories.OrderRepositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly ICompleteOrderWriteRepository _completeOrderWriteRepository;
        readonly ICompleteOrderReadRepository _completeOrderReadRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICompleteOrderWriteRepository completeOrderWriteRepository, ICompleteOrderReadRepository completeOrderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _completeOrderWriteRepository = completeOrderWriteRepository;
            _completeOrderReadRepository = completeOrderReadRepository;
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

           var data2 =  from order in data
            join completeOrder in _completeOrderReadRepository.Table
            on order.Id equals completeOrder.OrderId into co
            from _co in co.DefaultIfEmpty()
            select new
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate,
                OrderCode = order.OrderCode,
                Basket = order.Basket,
                Address = order.Address,
                Completed = _co != null ? true : false
            };



            return new()
            {
                TotalCount = await query.CountAsync(),
                Orders = await data2.Select(order => new
                {

                    Id = order.Id,
                    CreatedDate = order.CreatedDate,
                    OrderCode = order.OrderCode,
                    TotalPrice = order.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    UserName = order.Basket.User.UserName,
                    Address = order.Address,
                    order.Completed
                }).ToListAsync()

            };

        }

        public async Task<SingleOrder> GetByIdOrderAsync(string id)
        {

            //order tablosunun içindeki baskete gittik.sonra basketin içindeki basketitemslere .daha sonra basketitemsler içerisindeki productlara
            var data = _orderReadRepository.Table.Include(o => o.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);

            var data2 = await (from order in data
                        join completeOrder in _completeOrderReadRepository.Table
                                on order.Id equals completeOrder.OrderId into co
                         from _co in co.DefaultIfEmpty()
                         select new
                         {
                             Id = order.Id,
                             CreatedDate = order.CreatedDate,
                             OrderCode = order.OrderCode,
                             Description = order.Description,
                             Address = order.Address,
                             Completed = _co !=null ? true : false,
                             Basket = order.Basket,
                         }).FirstOrDefaultAsync(order => order.Id == Guid.Parse(id));

            return new() {
                Id = data2.Id.ToString(),
                BasketItems = data2.Basket.BasketItems.Select(bi => new // kaç tane basket items varsa hepsini getirir.
                {
                    bi.Product.Name,
                    bi.Product.Price,
                    bi.Quantity,
                }),
                Address = data2.Address ,
                CreatedDate= data2.CreatedDate,
                OrderCode = data2.OrderCode,
                Description = data2.Description,
                Completed = data2.Completed
               
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

        public  async Task CompleteOrderAsync(string orderId)
        {
            Order order = await _orderReadRepository.GetByIdAsync(orderId);

            if(order != null)
            {
               await _completeOrderWriteRepository.AddAsync(new() { OrderId = Guid.Parse(orderId)});
               await _completeOrderWriteRepository.SaveAsync(); 
            }

        }

    }
}
