using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.VerifyResetPasswordToken
{
    public class VerifyResetPasswordTokenCommandRequest : IRequest<VerifyResetPasswordTokenCommandResponse> 
    {
        public string UserId { get; set; }
        public string ResetToken { get; set; }
    }
}
