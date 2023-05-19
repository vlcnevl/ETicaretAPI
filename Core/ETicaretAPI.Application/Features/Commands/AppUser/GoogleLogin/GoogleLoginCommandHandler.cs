﻿using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    { // todo bu işlemi infranstucture da yapıp bir interface ile burda kullanmak daha doğru

        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        public GoogleLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { "59853966872-m1d7fjkbsc1m887ldu8evnni7759726p.apps.googleusercontent.com" }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);

            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        NameSurname = payload.Name,
                    };
                    var identityResult = await _userManager.CreateAsync(user); // asp.net users tablosuna kayıt
                    result = identityResult.Succeeded;

                }
            }

            if (result)
                await _userManager.AddLoginAsync(user, info);// Asp.NetUserLogins tablosuna eklendi.

            else
                throw new Exception("Google ile giriş işlemi başarısız.");

          
            Token token =  _tokenHandler.CreateAccessToken(5);
            return new()
            {
                Token = token
            };


        }
    }
}
