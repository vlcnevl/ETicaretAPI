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
           IdentityResult result = await _roleManager.CreateAsync(new() { Name = roleName });
           return result.Succeeded;
        }

        public async Task<bool> DeleteRole(string roleName)
        {
            IdentityResult result =  await _roleManager.DeleteAsync(new() { Name = roleName});
            return result.Succeeded;
        }

        public IDictionary<string, string> GetAllRoles()
        {
            return _roleManager.Roles.ToDictionary(role=> role.Id,role=> role.Name);

        }

        public async Task<(string id, string name)> GetByIdRole(string id)
        {
            string role = await _roleManager.GetRoleIdAsync(new() { Id = id});

            return (id,role);
        }

        public async Task<bool> UpdateRole(string id,string roleName)
        {
            IdentityResult result = await _roleManager.UpdateAsync(new() { Id = id, Name = roleName });
            return result.Succeeded;
        }
    }
}
