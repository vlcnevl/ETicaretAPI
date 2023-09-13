using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.Abstraction.Services.Configurations;
using ETicaretAPI.Application.Repositories.EndpointRepositories;
using ETicaretAPI.Application.Repositories.MenuRepositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services;

public class AuthorizationEndpointService : IAuthorizationEndpointService
{
    readonly IApplicationService  _applicationService;
    readonly IEndpointReadRepository _endpointReadRepository;
    readonly IEndpointWriteRepository _endpointWriteRepository;
    readonly IMenuReadRepository _menuReadRepository;
    readonly IMenuWriteRepository _menuWriteRepository;
    readonly RoleManager<AppRole> _roleManager;
    public AuthorizationEndpointService(IApplicationService applicationService, IEndpointReadRepository endpointReadRepository, IEndpointWriteRepository endpointWriteRepository, IMenuReadRepository menuReadRepository, IMenuWriteRepository menuWriteRepository, RoleManager<AppRole> roleManager)
    {
        _applicationService = applicationService;
        _endpointReadRepository = endpointReadRepository;
        _endpointWriteRepository = endpointWriteRepository;
        _menuReadRepository = menuReadRepository;
        _menuWriteRepository = menuWriteRepository;
        _roleManager = roleManager;
    }

    public async Task AssignRoleEndpointAsync(string[] roles, string code,string menu, Type type)
    {
        Menu? _menu = await _menuReadRepository.GetSingleAsync(o => o.Name == menu);
        if (_menu == null)
        {
         await _menuWriteRepository.AddAsync(new()
            {
                Id= Guid.NewGuid(),
                Name = menu,
            });
        }

        await _menuWriteRepository.SaveAsync();


        Endpoint? endpoint = await _endpointReadRepository.Table.Include(e=>e.Menu).FirstOrDefaultAsync(e=> e.Code == code && e.Menu.Name ==menu);

        if (endpoint == null)
        {
         var action = _applicationService.GetAuthhorizeDefinitionEndpoints(type).FirstOrDefault(m => m.Name == menu)?.Actions.FirstOrDefault(e=> e.Code ==code);


            endpoint = new()
            {
                Id = Guid.NewGuid(),
                Code = action.Code,
                ActionType = action.ActionType,
                HttpType = action.HttpType,
                Definition = action.Definition,

            };

           await _endpointWriteRepository.AddAsync(endpoint);
            
           await _endpointWriteRepository.SaveAsync();

        }

       var appRoles = await _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToListAsync();

        foreach(var role in appRoles)
            endpoint.Roles.Add(role);
        
        await _endpointWriteRepository.SaveAsync();
    }
}

