using ETicaretAPI.Application.Repositories.CompleteOrderRepositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Repositories.CompleteOrderRepositories
{
    public class CompleteOrderReadRepository : ReadRepository<CompletedOrder>, ICompleteOrderReadRepository
    {
        public CompleteOrderReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
