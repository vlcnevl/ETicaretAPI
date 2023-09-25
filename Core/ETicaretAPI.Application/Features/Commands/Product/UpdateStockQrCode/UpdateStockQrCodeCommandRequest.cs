using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateStockQrCode
{
    public class UpdateStockQrCodeCommandRequest : IRequest<UpdateStockQrCodeCommandResponse>
    {
        public string ProductId { get; set; }
        public int Stock { get; set; }
    }
}
