using ETicaretAPI.Application.Abstraction.GoogleLogin;
using ETicaretAPI.Application.Abstraction.Stroage;
using ETicaretAPI.Application.Abstraction.Stroage.LocalStroage;
using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Infrastructure.Enums;
using ETicaretAPI.Infrastructure.Services;
using ETicaretAPI.Infrastructure.Services.GoogleLogin;
using ETicaretAPI.Infrastructure.Services.Stroage;
using ETicaretAPI.Infrastructure.Services.Stroage.Azure;
using ETicaretAPI.Infrastructure.Services.Stroage.Local;
using ETicaretAPI.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
           services.AddScoped<IStroageService,StroageService>();
           services.AddScoped<ITokenHandler, TokenHandler>();
           services.AddScoped<IGoogleLogin, GoogleLogin>();
        }

        public static void AddStroage<T>(this IServiceCollection services) where T : Stroage, IStroage
        {
            services.AddScoped<IStroage, T>();
        }
        //yukarıdaki daha  tercih edilen bir kod.
        public static void AddStroage<T>(this IServiceCollection services,StroageType stroageType)
        {
            switch(stroageType)
            {
                case StroageType.Local: services.AddScoped<IStroage, LocalStroage>(); break;
                case StroageType.Azure: services.AddScoped<IStroage, AzureStroage>(); break;

                default : services.AddScoped<IStroage, LocalStroage>(); break;
            }

           
        }
    }
}
