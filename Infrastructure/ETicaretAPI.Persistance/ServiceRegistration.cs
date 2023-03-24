using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Persistance.Concretes;
using Microsoft.Extensions.DependencyInjection;


namespace ETicaretAPI.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddPersistanceServices(this IServiceCollection services) // IoC ye eklemek için extension func yazdık değer. yapınca gelsin diye
        {
            services.AddSingleton<IProductService, ProductService>();
        }


    }
}
