using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Reflection;

namespace ETicaretAPI.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services) 
        {
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly));
        }
    }
}
