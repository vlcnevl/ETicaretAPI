using ETicaretAPI.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services.Authentications
{
    public interface IInternalAuthentication // solid için ayırdık .
    {
        Task<DTOs.Token> LoginAsync(LoginUser model, int accessTokenLifeTime); // internal login 
    }
}
