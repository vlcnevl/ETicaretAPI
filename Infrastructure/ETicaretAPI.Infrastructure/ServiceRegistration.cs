using ETicaretAPI.Application.Abstraction.Stroage;
using ETicaretAPI.Application.Abstraction.Stroage.LocalStroage;
using ETicaretAPI.Infrastructure.Enums;
using ETicaretAPI.Infrastructure.Services;
using ETicaretAPI.Infrastructure.Services.Stroage;
using ETicaretAPI.Infrastructure.Services.Stroage.Local;
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
        }

        public static void AddStroage<T>(this IServiceCollection services) where T : class, IStroage
        {
            services.AddScoped<IStroage, T>();
        }
        public static void AddStroage<T>(this IServiceCollection services,StroageType stroageType)
        {
            switch(stroageType)
            {
                case StroageType.Local: services.AddScoped<IStroage, LocalStroage>(); break;
                case StroageType.Azure: break;

                default : services.AddScoped<IStroage, LocalStroage>(); break;
            }

           
        }
    }
}
