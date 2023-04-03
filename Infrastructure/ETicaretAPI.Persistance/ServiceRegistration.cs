using ETicaretAPI.Persistance.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ETicaretAPI.Application.Repositories.CustomerRepositories;
using ETicaretAPI.Persistance.Repositories.CustomerRepositories;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using ETicaretAPI.Persistance.Repositories.ProductRepositories;
using ETicaretAPI.Application.Repositories.OrderRepositories;
using ETicaretAPI.Persistance.Repositories.OrderRepositories;

namespace ETicaretAPI.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddPersistanceServices(this IServiceCollection services) // IoC ye eklemek için extension func yazdık değer. yapınca gelsin diye
        {

            //autofac ile güncellenebilir.
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString));
            services.AddScoped<ICustomerReadRepository,CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository,CustomerWriteRepository>();
            services.AddScoped<IProductReadRepository,ProductReadRepository>();
            services.AddScoped<IProductWriteRepository,ProductWriteRepository>();
            services.AddScoped<IOrderReadRepository,OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository,OrderWriteRepository>();

        }


    }
}
