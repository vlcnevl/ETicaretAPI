using ETicaretAPI.Persistance.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ETicaretAPI.Application.Repositories.CustomerRepositories;
using ETicaretAPI.Persistance.Repositories.CustomerRepositories;
using ETicaretAPI.Application.Repositories.ProductRepositories;
using ETicaretAPI.Persistance.Repositories.ProductRepositories;
using ETicaretAPI.Application.Repositories.OrderRepositories;
using ETicaretAPI.Persistance.Repositories.OrderRepositories;
using ETicaretAPI.Application.Repositories.File;
using ETicaretAPI.Persistance.Repositories.FileRepositories;
using ETicaretAPI.Application.Repositories.ProductIamgeFileRepositories;
using ETicaretAPI.Persistance.Repositories.ProductImageFileRepositories;
using ETicaretAPI.Application.Repositories.InvoiceFileRepositories;
using ETicaretAPI.Persistance.Repositories.InvoiceFileRepositories;
using ETicaretAPI.Domain.Entities.Identity;

namespace ETicaretAPI.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddPersistanceServices(this IServiceCollection services) // IoC ye eklemek için extension func yazdık değer. yapınca gelsin diye
        {

            //autofac ile güncellenebilir.
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString));
            services.AddIdentity<AppUser, AppRole>(options=> { 
                options.Password.RequiredLength = 3; // şifre sınrlamalarını kaldırdım.
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ETicaretAPIDbContext>();//identity mekanizması
            
            services.AddScoped<ICustomerReadRepository,CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository,CustomerWriteRepository>();
            services.AddScoped<IProductReadRepository,ProductReadRepository>();
            services.AddScoped<IProductWriteRepository,ProductWriteRepository>();
            services.AddScoped<IOrderReadRepository,OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository,OrderWriteRepository>();
            services.AddScoped<IFileWriteRepository, FileWriteRepository>();
            services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddScoped<IProductImageFileReadRepository, ProductImageFileReadRepository>();
            services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();
            services.AddScoped<IInvoiceFileReadRepository, InvoiceFlieReadRepository>();
            services.AddScoped<IInvoiceFileWriteRepository, InvoiceFileWriteRepository>();

        }


    }
}
