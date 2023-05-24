using Microsoft.AspNetCore.Identity; // bu paket .net6 da Microsoft.Extensions.Identity.Stores kütüphanesinin soyutlaması sayesinde kullanılabiliyor.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities.Identity
{
    public class AppUser : IdentityUser<string>
    {
        public string NameSurname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenEndTime { get; set; }
    }
}
