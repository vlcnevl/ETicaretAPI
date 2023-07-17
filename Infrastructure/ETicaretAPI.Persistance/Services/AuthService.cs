using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class AuthService : IAuthService
    {

        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
        readonly IConfiguration _configuration;
        readonly IUserService _userService;
        readonly IMailService _mailService;
        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, SignInManager<AppUser> signInManager, IConfiguration configuration, IUserService userService, IMailService mailService)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _configuration = configuration;
            _userService = userService;
            _mailService = mailService;
        }

        public async Task<Token> GoogleLoginAsync(string idToken,int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["Google:GoogleId"] }
            };


            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            var info = new UserLoginInfo("GOOGLE", payload.Subject,"GOOGLE");

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
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
            {
                await _userManager.AddLoginAsync(user, info);// Asp.NetUserLogins tablosuna eklendi.
                Application.DTOs.Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime,user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration,600);

                return token;
            }
            else    
            throw new Exception("Google ile giriş işlemi başarısız.");
        }

        public async Task<Token> LoginAsync(LoginUser model, int accessTokenLifeTime)
        {
            // ilk başta kullanıcı varmı diye kontrol edilir eğer varsa kullanıcının şifresi kontrol edilir.
            Domain.Entities.Identity.AppUser? user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
                throw new UserNotFoundException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false); //kullanıcı ile şifreyi karşılaştırmak için. 3 kere yanlış şifre girilince beş dk beklenmesin
            if (result.Succeeded) //authantication basarili
            {
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 2400);//refresh tokeni olustur ve veritabanına kaydet.

                return token; 
            }

            //  return new LoginUserErrorCommandResponse() { Message = "Kimlik doğrulama hatası"};
            throw new AuthenticationErrorException();
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
           AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u=> u.RefreshToken ==  refreshToken);
            //gelen refresh tokeni içeren satır varsa getirdi.
            if (user != null && user?.RefreshTokenEndTime > DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccessToken(15,user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 2400);
                return token;
            }
            else
                throw new UserNotFoundException();
        }

        public async Task ResetPasswordAsync(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if(user != null)
            {
                
              var token = await _userManager.GeneratePasswordResetTokenAsync(user);
              token = CustomEncoders.UrlEncode(token); //custom helpers yazdık 
              await _mailService.SendPasswordResetMailAsync(email, user.Id, token);
             
            }
        }

        public async Task<bool> VerifyResetPasswordToken(string userId, string resetToken)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
               resetToken =  CustomEncoders.UrlDecode(resetToken);

                return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);

            }

            return false;
        }
    }
}
