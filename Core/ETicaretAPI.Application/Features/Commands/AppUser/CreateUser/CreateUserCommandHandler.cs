using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public CreateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
           IdentityResult result = await _userManager.CreateAsync(new() 
            { 
               Id = Guid.NewGuid().ToString(), // identity kendi değer veremediği için biz verdik
               Email = request.Email,
               NameSurname = request.NameSurname,
               UserName = request.Username
            },request.Password);

            CreateUserCommandResponse response = new() { Succeded = result.Succeeded};
            if (result.Succeeded)
                 response.Message="Kullanıcı başarıyla oluşturuldu";
            else
                foreach(var error in result.Errors)
                    response.Message += $"{error.Code} - {error.Description}<br>";


            return response;   
         //  throw new UserCreateFailedException("Kullanıcı oluşturulurken hata oluştu");
        }
    }
}
