using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Stroage.Azure;
using ETicaretAPI.Infrastructure.Services.Stroage.Local;
using ETicaretAPI.Persistance;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistanceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddStroage<AzureStroage>();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy=> policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()));//CORS POL�T�KASI

builder.Services.AddControllers(options=> options.Filters.Add<ValidationFilter>()).AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
        .ConfigureApiBehaviorOptions(options=> options.SuppressModelStateInvalidFilter = true); 
// application katman�nda bir yer belli etmek i�in verdiik oraya ka� tane validator eklersek eklenecek hepsi
//ikinci options asp.net le gelen base validasyon filtrelerini kald�rarak bizim validasyonlar� yazacak

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // wwwroot pathine dosya y�kleyebilmek i�in
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
