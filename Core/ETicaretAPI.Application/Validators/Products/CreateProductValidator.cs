using ETicaretAPI.Application.ViewModels.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<CreateProductViewModel>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty().NotNull().WithMessage("Lütfen ürün adını boş bırakmayınız.")
                .MaximumLength(150).MinimumLength(5).WithMessage("Lütfen ürün adını 5 ile 150 karakter arasında giriniz.");

            RuleFor(p => p.Stock).NotNull().NotEmpty().WithMessage("Lütfen stok bilgisini boş bırakmayınız.")
                .Must(s => s >= 0).WithMessage("Stok bilgisi negatif olamaz");


            RuleFor(p => p.Price).NotNull().NotEmpty().WithMessage("Lütfen fiyat bilgisini boş bırakmayınız.")
                .Must(p => p >= 0).WithMessage("Fiyat bilgisi negatif olamaz");


            RuleFor(p => p.Description).NotEmpty().NotNull().WithMessage("Lütfen açıklama kısmını boş bırakmayınız.")
               .MaximumLength(150).MinimumLength(5).WithMessage("Lütfen ürün açıklamasını 5 ile 150 karakter arasında giriniz.");

        }

    }
}
