using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Features.Commands.AppUser.UpdatePassword;
using ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint;
using ETicaretAPI.Application.Features.Queries.AppUser.GetAllUser;
using ETicaretAPI.Application.Features.Queries.AppUser.GetRolesToUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IMediator _mediator;
        public UsersController(IMediator mediator, IMailService mailService)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommandRequest request)
        {
            CreateUserCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest request)
        {
            UpdatePasswordCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes ="Admin")]
        [AuthorizeDefinition(ActionType = Application.Enums.ActionType.Reading, Definition = "Get all users", Menu = "Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUserQueryRequest request)
        {
            GetAllUserQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }


        [HttpGet("get-roles-to-user/{UserId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = Application.Enums.ActionType.Reading, Definition = "Get roles to user", Menu = "Users")]
        public async Task<IActionResult> GetRolesToUser([FromRoute] GetRolesToUserQueryRequest request)
        {
           GetRolesToUserQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("assign-role-to-user")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = Application.Enums.ActionType.Writing, Definition = "Assign role to user", Menu = "Users")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleEndpointCommandRequest request)
        {
            AssignRoleEndpointCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

    }
}
