using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services
{
    public interface IRoleService
    {
        Task<bool> CreateRole(string roleName);
        Task<bool> DeleteRole(string Id);
        Task<bool> UpdateRole(string id,string roleName);
        (object,int) GetAllRoles(int page,int size);
        Task<(string id,string name)> GetByIdRole(string id);
    }
}
