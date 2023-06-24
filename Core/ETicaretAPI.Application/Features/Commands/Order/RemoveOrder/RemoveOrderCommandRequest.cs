﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.RemoveOrder
{
    public class RemoveOrderCommandRequest : IRequest<RemoveOrderCommandResponse>
    {
        public string OrderId { get; set; }
    }
}
