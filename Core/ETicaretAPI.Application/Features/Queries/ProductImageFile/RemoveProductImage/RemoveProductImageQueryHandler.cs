﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.ProductImageFile.RemoveProductImage
{
    public class RemoveProductImageQueryHandler : IRequestHandler<RemoveProductImageQueryRequest, RemoveProductImageQueryResponse>
    {
        public Task<RemoveProductImageQueryResponse> Handle(RemoveProductImageQueryRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
