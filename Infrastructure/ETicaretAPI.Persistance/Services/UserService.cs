using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Application.Repositories.EndpointRepositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly IEndpointReadRepository _endpointReadRepository;
        public UserService(UserManager<AppUser> userManager, IEndpointReadRepository endpointReadRepository)
        {
            _userManager = userManager;
            _endpointReadRepository = endpointReadRepository;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUser model)
        {

            IdentityResult result = await _userManager.CreateAsync(new ()
            {
                Id = Guid.NewGuid().ToString(), 
                Email = model.Email,
                NameSurname = model.NameSurname,
                UserName = model.Username
            }, model.Password);

            CreateUserResponse response = new() { Succeded = result.Succeeded };
            if (result.Succeeded)
                response.Message = "Kullanıcı başarıyla oluşturuldu";
            else
                foreach (var error in result.Errors)
                    response.Message += $"{error.Code} - {error.Description}<br>";
           
            return response;
        }

        public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenTime,int refreshTokenLifeTime)
        {
            if (user!=null) 
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndTime = accessTokenTime.AddSeconds(refreshTokenLifeTime);
                await _userManager.UpdateAsync(user);
            }
            else
            throw new UserNotFoundException();
        }


        public async Task UpdatePasswordAsync(string userId, string newPassword, string resetToken)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
               resetToken = resetToken.UrlDecode();
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
           

                if(result.Succeeded)
                { //reset token security stamp ile doğrulandığı için parola güncellenince security stampda güncellendi.reset linki işlevsizleşti.
                   await _userManager.UpdateSecurityStampAsync(user);
                }
                else throw new PasswordChangeFailedException();
            
            }

        }

        public async Task<List<ListUser>> GetAllUsersAsync(int page,int size)
        {
          var users = await _userManager.Users.Skip(page*size).Take(size).ToListAsync();

            return users.Select(u => new ListUser
            {   Id= u.Id,
                Email= u.Email,
                NameSurname = u.NameSurname,
                Username = u.UserName
            }).ToList();


        }

        public async Task AssignRoleToUserAsync(string userId, string[] roles)
        {
            AppUser user =await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
              var _roles = await _userManager.GetRolesAsync(user);
               await _userManager.RemoveFromRolesAsync(user, _roles);

               await _userManager.AddToRolesAsync(user, roles);
            }

        }

        public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
        {
           AppUser user = await _userManager.FindByIdAsync(userIdOrName);

            if(user == null) 
            {
                user = await _userManager.FindByNameAsync(userIdOrName);
            }

            if(user != null)
            {
               var userRoles = await _userManager.GetRolesAsync(user);

                return userRoles.ToArray();
            }
            
            throw new Exception("Kullanıcı bulunamadı");// özel bir hatada fırlatılabilir.
        }

        public async Task<bool> HasRolePermissionToEndpointAsync(string name, string endpointCode)
        {
            //endpointlere atanmıs roller ile kullanıcıya atanmıs rolleri karsılastırıyoruz.
            var userRoles = await GetRolesToUserAsync(name);

            if (!userRoles.Any()) 
                return false;

             Endpoint? endpoint = await _endpointReadRepository.Table.Include(e => e.Roles).FirstOrDefaultAsync(e=> e.Code == endpointCode);

            if (endpoint == null)
                return false;

            var hasRole = false;

            var endpointRoles = endpoint.Roles.Select(e => e.Name).ToArray();

            hasRole = endpointRoles.Intersect(userRoles).Count()>0 ? true : false ;

            return hasRole;
        }
    }
}

