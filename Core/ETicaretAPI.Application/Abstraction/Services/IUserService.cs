using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateAsync(CreateUser model); // cqrs modelini dto ya çevirip gönderdik.gelecek olan dto yu da cqrs responsa döndürceğiz.
        Task UpdateRefreshTokenAsync(string refreshToken,AppUser user,DateTime accessTokenTime, int refreshTokenLifeTime);
        Task UpdatePasswordAsync(string userId, string newPassword, string resetToken);
        Task<List<ListUser>> GetAllUsersAsync(int page,int size);
        Task AssignRoleToUser(string userId, string[] roles);
        Task<string[]> GetRolesToUser(string userId);
    
    }
}
