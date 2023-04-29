using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IReadRepository<T> : IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll(bool tracking = true); // sorgular direkt veritabanında 
        IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true); // şart ifadesi doğru olan datalar gelecek
        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true);
        Task<T> GetByIdAsync(string id, bool tracking = true);
       
    }
}
