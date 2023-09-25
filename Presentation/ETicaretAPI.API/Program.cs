using ETicaretAPI.API.Configurations.ColumnWriters;
using ETicaretAPI.API.Extenions;
using ETicaretAPI.API.Filters;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Stroage.Azure;
using ETicaretAPI.Persistance;
using ETicaretAPI.SignalR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();//clientten gelen request sonucunda olusan httpContext nesnesini katmanlardaki classlarda (bussiness logic) erismemisi saðlar.
//katmanlarda gelen userin bilgilerini kullanabilmek için.
builder.Services.AddPersistanceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddSignalRServices();
builder.Services.AddStroage<AzureStroage>();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy=> policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials() ));//CORS POLÝTÝKASI

builder.Services.AddControllers(options=> // filtreleri pipeline a ekledik.
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<RolePermissionFilter>();   
}).AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
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
        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false, //tokeni expire edecek.
     
        NameClaimType = ClaimTypes.Name, //hangi kullanýcýnýn istek yaptýðýný gösterir . logda usernamei kullanabilmek için User.Identity.Name den kullanabiliriz.

    };
    
});

Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSql"), "logs", needAutoCreateTable: true,
    columnOptions: new Dictionary<string, ColumnWriterBase> //veritabanýndaki log tablosu düzenlendi.
    {
        {"message",new RenderedMessageColumnWriter()},
        {"message_template",new MessageTemplateColumnWriter()},
        {"level",new LevelColumnWriter()},
        {"time_stamp",new TimestampColumnWriter()},
        {"exception",new ExceptionColumnWriter()},
        {"log_event",new LogEventSerializedColumnWriter()},
        {"user_name", new UsernameColumnWriter()}

    })
    .Enrich.FromLogContext()//logcontexte asagida koydugumuz propertiyi burda kullanacaðýz dedik.
    .MinimumLevel.Information()
    .CreateLogger();
    
builder.Host.UseSerilog(log);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua"); //kullanýcýnýn bütün bilgilerini getiir.
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build(); //middlewaresler

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());// middleware a göndermek için bir logger nesnesini gönderdik

app.UseStaticFiles(); // wwwroot pathine dosya yükleyebilmek için
app.UseSerilogRequestLogging();// bu middlewareden sonra ne varsa loglansýn
app.UseHttpLogging();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication(); // authenticationu yukarda ekledik burda da kontrol ettiriiyoruz
app.UseAuthorization();


app.Use(async (context,next) => //burda yazýlmasý pek dogru degil.
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null; // gelen istekteki jwt ye bakar. içerisinde username varsa getirir.
    LogContext.PushProperty("user_name", username);

    await next();
});//middleware authenticate olmus userin ismini getirecek ve loglama yapacaðýz.



app.MapControllers();
app.MapHubs(); // signalR classlibden gelen extension fonksiyon.

app.Run();
