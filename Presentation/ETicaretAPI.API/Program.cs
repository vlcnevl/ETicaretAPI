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

builder.Services.AddCors(options => options.AddDefaultPolicy(policy=> policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()));//CORS POLÝTÝKASI

builder.Services.AddControllers(options=> options.Filters.Add<ValidationFilter>()).AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
        .ConfigureApiBehaviorOptions(options=> options.SuppressModelStateInvalidFilter = true);
// application katmanýnda bir yer belli etmek için verdiik oraya kaç tane validator eklersek eklenecek hepsi
//ikinci options asp.net le gelen base validasyon filtrelerini kaldýrarak bizim validasyonlarý yazacak


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Admin",options =>
{
    options.TokenValidationParameters = new() // hangi deðerlere göre token doðrulanacak
    {
        ValidateAudience = true, // token deðerini kimlerin hangi sitelerin kullanacaðýný belirleriz. www. ... . com
        ValidateIssuer = true, // token deðerini kimin daðýttýðýný ifade eder   www.myapi.com bizim api
        ValidateLifetime = true, // token deðerinin süresini kontrol eder.
        ValidateIssuerSigningKey = true, // token deðerinin uygulamaya ait olduðunu belli eden security key.

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

app.UseStaticFiles(); // wwwroot pathine dosya yükleyebilmek için
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication(); // authenticationu yukarda ekledik burda da kontrol ettiriiyoruz
app.UseAuthorization();

app.MapControllers();

app.Run();
