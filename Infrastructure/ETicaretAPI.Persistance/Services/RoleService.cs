using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class RoleService : IRoleService
    {
        readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRole(string roleName)
        {
           IdentityResult result = await _roleManager.CreateAsync(new() { Id=Guid.NewGuid().ToString() ,Name = roleName });
           return result.Succeeded;
        }

        public async Task<bool> DeleteRole(string Id)
        {
            AppRole role = await _roleManager.FindByIdAsync(Id);
            IdentityResult result =  await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public (object,int) GetAllRoles(int page,int size)
        {
            var query = _roleManager.Roles;

            IQueryable<AppRole> resultQuery = null;
            if (page != -1 && size != -1)
                resultQuery = query.Skip(page * size).Take(size);
            else
                resultQuery = query;

            return (resultQuery.Select(r=> new {r.Id,r.Name}), query.Count());
        }

        public async Task<(string id, string name)> GetByIdRole(string id)
        {
            string role = await _roleManager.GetRoleIdAsync(new() { Id = id});

            return (id,role);
        }

        public async Task<bool> UpdateRole(string id,string roleName)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            role.Name = roleName;
            IdentityResult result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }
    }
}
