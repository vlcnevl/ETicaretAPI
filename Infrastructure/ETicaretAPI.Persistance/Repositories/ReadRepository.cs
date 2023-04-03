using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Common;
using ETicaretAPI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETicaretAPI.Persistance.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly ETicaretAPIDbContext _context;

        public ReadRepository(ETicaretAPIDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = Table.AsQueryable();

            if (!tracking) // tracke  edilmesini istemiyorsak 
                query = query.AsNoTracking();

            return query;

        }

        public async Task<T> GetByIdAsync(string id, bool tracking = true)

        //=> await Table.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id));
        //=> await Table.FindByIdAsync(Guid.Parse(id));
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = Table.AsNoTracking();
            return await query.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id));

        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true)
        {
           var query = Table.AsQueryable();

            if (!tracking)
                query = Table.AsNoTracking();

            return await query.FirstOrDefaultAsync(method);
        }
            
            

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = Table.Where(method);
            
            if(!tracking)
                query = query.AsNoTracking();

            return query;

        }
    }
}
