using ETicaretAPI.Application.Abstraction.Services.Hubs;
using ETicaretAPI.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ETicaretAPI.SignalR.HubService
{
    //hubu mimarinin herhangi bir  yerine inject edip kullanavilmek için. interfacei application da .
    public class ProductHubService : IProductHubService
    {

        readonly IHubContext<ProductHub> _hubContext;

        public ProductHubService(IHubContext<ProductHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task ProductAddedMessageAsync(string message)
        {
           await _hubContext.Clients.All.SendAsync(ReceiveFunctionNames.ProductAddedMessage,message);
        }
    }
}
