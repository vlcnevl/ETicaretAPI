using ETicaretAPI.Application.Abstraction.Services.Hubs;
using ETicaretAPI.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.SignalR.HubService
{
    public class OrderHubService : IOrderHubService
    {
        readonly IHubContext<OrderHub> _context;

        public OrderHubService(IHubContext<OrderHub> context)
        {
            _context = context;
        }

        public async Task OrderAddedMessageAsync(string message)
        {
            await _context.Clients.All.SendAsync(ReceiveFunctionNames.OrderAddedMessage,message);
        }
    }
}
