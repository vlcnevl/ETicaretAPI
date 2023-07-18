using ETicaretAPI.Domain.Entities;
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
        public DbSet<CompletedOrder> CompletedOrders { get; set; }


        protected override void OnModelCreating(ModelBuilder builder) // order ve basket arasında 1-1 iliski oldugu için bunu bildirmemiz gerekli.
        {
            builder.Entity<Order>().HasKey(o => o.Id); // orderdaki idyi primary key olarak ayarladık.

            builder.Entity<Order>().HasIndex(o => o.OrderCode).IsUnique(); // ordercode benzersiz bir değer olsun.

            builder.Entity<Basket>().HasOne(b => b.Order).WithOne(o => o.Basket).HasForeignKey<Order>(o => o.Id);
            //orderin içindeki id foreign key

            builder.Entity<Order>().HasOne(o => o.CompletedOrder).WithOne(c => c.Order).HasForeignKey<CompletedOrder>(o => o.OrderId);
            //complete order tablosubnda olan orderId order'in id si ile eşleşti.




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
