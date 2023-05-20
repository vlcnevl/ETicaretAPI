using ETicaretAPI.Application.Abstraction.GoogleLogin;
using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.GoogleLogin
{
    public class GoogleLogin : IGoogleLogin
    {

        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;

        public GoogleLogin(UserManager<AppUser> userManager, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
        }

        public async Task<Application.DTOs.Token> Login(GoogleLoginCommandRequest request)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { "59853966872-m1d7fjkbsc1m887ldu8evnni7759726p.apps.googleusercontent.com" }
            };


            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);

            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            
            bool result = user != null;

            if (user==null) 
            {
                user = await _userManager.FindByEmailAsync(payload.Email); 
            
                if (user==null) 
                {
                    user = new()
                    {
                        Id= Guid.NewGuid().ToString(),
                        Email= payload.Email,
                        UserName =payload.Email,
                        NameSurname = payload.Name,
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
                await _userManager.AddLoginAsync(user, info);// Asp.NetUserLogins tablosuna eklendi.

            else
                throw new Exception("Google ile giriş işlemi başarısız.");


            Application.DTOs.Token token = _tokenHandler.CreateAccessToken(5);
            return token;
        }
    }
}
