﻿using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = ETicaretAPI.Domain.Entities.File;

namespace ETicaretAPI.Persistance.Contexts
{
    public class ETicaretAPIDbContext : IdentityDbContext<AppUser,AppRole,string> // identity tabloları eklenmesi için
    {
        public ETicaretAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<File> Files { get; set; }   // bu ve alltaki ikisi tableperhiearchy olayı hepsi tek tabloda birleşti.
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }  // 
        public DbSet<Basket> Baskets { get; set; }  
        public DbSet<BasketItem> BasketItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder) // order ve basket arasında 1-1 iliski oldugu için bunu bildirmemiz gerekli.
        {
            builder.Entity<Basket>().HasKey(b => b.Id); // basketteki idyi primary key olarak ayarladık.

            builder.Entity<Basket>().HasOne(b => b.Order).WithOne(o => o.Basket).HasForeignKey<Basket>(b => b.OrderId);

            base.OnModelCreating(builder); //identityDbContext kullandığımız için override edemedik.basedekini çağırdık. 
        }



        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) // interceptor her savechanges tetiklendiğinde buraya gelecek 
        {
            //update de verinin yakalanmasını sağlar.track yapılan veriyi yakalar.
            var datas = ChangeTracker.Entries<BaseEntity>();

            foreach (var data in datas ) 
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow,
                    EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow,

                    _ => DateTime.UtcNow //silinen veri yukarıda eşleşmediği için hata aldık. silinen veri için böylesi iyi
                };
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
