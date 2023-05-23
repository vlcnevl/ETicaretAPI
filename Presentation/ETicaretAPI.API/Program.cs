using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Stroage.Azure;
using ETicaretAPI.Persistance;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Admin",options =>
{
    options.TokenValidationParameters = new() // hangi de�erlere g�re token do�rulanacak
    {
        ValidateAudience = true, // token de�erini kimlerin hangi sitelerin kullanaca��n� belirleriz. www. ... . com
        ValidateIssuer = true, // token de�erini kimin da��tt���n� ifade eder   www.myapi.com bizim api
        ValidateLifetime = true, // token de�erinin s�resini kontrol eder.
        ValidateIssuerSigningKey = true, // token de�erinin uygulamaya ait oldu�unu belli eden security key.

        ValidAudience = builder.Configuration["Token:Audience"],
        ValidIssuer = builder.Configuration["Token:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires!=null ? expires > DateTime.UtcNow : false //tokeni expire edecek.
     
    };
    
});
 
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

app.UseAuthentication(); // authenticationu yukarda ekledik burda da kontrol ettiriiyoruz
app.UseAuthorization();

app.MapControllers();

app.Run();
