using ETicaretAPI.Application.Abstraction.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
       readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Application.DTOs.Token CreateAccessToken(int minute)
        {
           Application.DTOs.Token token = new ();
            
            //security keyin simetriğini aldık
            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

            //şifrelenmiş security key olusturuyoruz.
            SigningCredentials signingCredentials = new(symmetricSecurityKey,SecurityAlgorithms.HmacSha256); 

            //token ayarları
            token.Expiration = DateTime.UtcNow.AddMinutes(minute);
            JwtSecurityToken securityToken = new
                ( audience : _configuration["Token:Audience"],
                  issuer: _configuration["Token:Issuer"],
                  expires:token.Expiration,
                  notBefore:DateTime.UtcNow, // üretildiği andan itibaren devreye girsin.
                  signingCredentials:signingCredentials
                );

            // token oluşturucu sınıf
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken( securityToken );

            return token;
        }
    }
}
