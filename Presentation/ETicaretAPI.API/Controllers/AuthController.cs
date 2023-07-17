﻿using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Features.Commands.AppUser.RefreshTokenLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.ResetPassword;
using ETicaretAPI.Application.Features.Commands.AppUser.VerifyResetPasswordToken;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginUserCommandRequest request)
        {
            LoginUserCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody]RefreshTokenLoginCommandRequest request)
        {
           RefreshTokenLoginCommandResponse response= await _mediator.Send(request);
           return Ok(response);
        }


        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginCommandRequest request)
        {
            GoogleLoginCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordCommandRequest request)
            {
            ResetPasswordCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("verify-reset-password-token")]
        public async Task<IActionResult> VerifyResetPasswordToken([FromBody] VerifyResetPasswordTokenCommandRequest request)
        {
            VerifyResetPasswordTokenCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

    }
}
