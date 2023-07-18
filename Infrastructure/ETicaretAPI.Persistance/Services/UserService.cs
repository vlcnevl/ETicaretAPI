using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
    }
}

