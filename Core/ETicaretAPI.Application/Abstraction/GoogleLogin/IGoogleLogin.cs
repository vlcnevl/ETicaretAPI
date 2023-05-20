using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.GoogleLogin
{
    public interface IGoogleLogin
    {
        Task<DTOs.Token> Login(GoogleLoginCommandRequest request);
    }
}
