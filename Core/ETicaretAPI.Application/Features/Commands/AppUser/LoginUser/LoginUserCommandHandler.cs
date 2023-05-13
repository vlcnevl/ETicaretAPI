using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
        public LoginUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, SignInManager<Domain.Entities.Identity.AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        { // ilk başta kullanıcı varmı diye kontrol edilir eğer varsa kullanıcının şifresi kontrol edilir.
           Domain.Entities.Identity.AppUser? user = await _userManager.FindByNameAsync(request.UsernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);

            if (user == null)
                throw new UserNotFoundException();

           SignInResult result =  await _signInManager.CheckPasswordSignInAsync(user, request.Password,false); //kullanıcı ile şifreyi karşılaştırmak için. 3 kere yanlış şifre girilince beş dk beklenmesin
            if (result.Succeeded)
            {
                //yetki islemleri
            }

            return new();
        }
    }
}
