using ETicaretAPI.Application.Repositories.InvoiceFileRepositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Repositories.InvoiceFileRepositories
{
    public class InvoiceFlieReadRepository : ReadRepository<InvoiceFile>, IInvoiceFileReadRepository
    {
        public InvoiceFlieReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
