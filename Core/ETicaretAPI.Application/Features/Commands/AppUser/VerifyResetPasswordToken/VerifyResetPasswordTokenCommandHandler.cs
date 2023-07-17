using ETicaretAPI.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.VerifyResetPasswordToken
{
    public class VerifyResetPasswordTokenCommandHandler : IRequestHandler<VerifyResetPasswordTokenCommandRequest, VerifyResetPasswordTokenCommandResponse>
    {
        readonly IAuthService _authService;

        public VerifyResetPasswordTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<VerifyResetPasswordTokenCommandResponse> Handle(VerifyResetPasswordTokenCommandRequest request, CancellationToken cancellationToken)
        {
          bool state =  await _authService.VerifyResetPasswordToken(request.UserId, request.ResetToken);
            return new() { State = state};
        }
    }
}
