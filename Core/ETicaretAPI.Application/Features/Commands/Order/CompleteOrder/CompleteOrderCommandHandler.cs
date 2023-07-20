using ETicaretAPI.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.CompleteOrder
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommandRequest, CompleteOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IMailService _mailService;
        public CompleteOrderCommandHandler(IOrderService orderService, IMailService mailService)
        {
            _orderService = orderService;
            _mailService = mailService;
        }

        public async Task<CompleteOrderCommandResponse> Handle(CompleteOrderCommandRequest request, CancellationToken cancellationToken)
        {
           (bool, Application.DTOs.Order.CompletedOrder) result = await _orderService.CompleteOrderAsync(request.Id);
           //(bool succeded, Application.DTOs.Order.CompletedOrder dto)  = await _orderService.CompleteOrderAsync(request.Id);


            if (result.Item1)
            {
                _mailService.SendCompletedOrderMailAsync(result.Item2.Email,result.Item2.NameSurname,result.Item2.OrderCode,result.Item2.OrderDate);
            }
            return new() { };
        }
    }
}
