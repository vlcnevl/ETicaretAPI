using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.InvoiceFileRepositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Repositories.InvoiceFileRepositories
{
    public class InvoiceFileWriteRepository : WriteRepository<InvoiceFile>, IInvoiceFileWriteRepository
    {
        public InvoiceFileWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
 