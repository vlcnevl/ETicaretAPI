using ETicaretAPI.Application.Abstraction.Services;
using ETicaretAPI.Application.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace ETicaretAPI.API.Filters
{
    public class RolePermissionFilter : IAsyncActionFilter
    {
        readonly IUserService _userService;

        public RolePermissionFilter(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var name = context.HttpContext.User.Identity?.Name;

            if(!string.IsNullOrEmpty(name) && name!="vlcnevl") // gömülü olarak admin vlcnevl kullanıcısı ayarlandı.
            {
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
                //controllerdaki attribute'in bilgilerini öğrendik.
                var attribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;

                //metodun get post put delete mi oldugunu öğrendik.
                var httpAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;

                //bu bilgilerle attribute ile veridğimiz kodların aynısını olusturup eslestirme yapacağız.
                //actionlar herhangi bir http attiribute ile işaretlenmediyse get metodu olarak işaretlenir.
                var code = $"{(httpAttribute != null ? httpAttribute.HttpMethods.First() : HttpMethods.Get)}.{attribute.ActionType}.{attribute.Definition.Replace(" ", "")}";

                var hasRole = await _userService.HasRolePermissionToEndpointAsync(name,code);
                if (!hasRole)
                    context.Result = new UnauthorizedResult();   
                
                else
                    await next();

            }
            else
             await next(); // kullanıcı ile ilgili herhangi bir rol gelmediyse diğer actionlara devam et

        }
    }
}
