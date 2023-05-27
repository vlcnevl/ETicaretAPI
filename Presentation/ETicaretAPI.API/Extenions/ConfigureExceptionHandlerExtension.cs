using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net;
using System.Net.Mime;

namespace ETicaretAPI.API.Extenions
{
    static public class ConfigureExceptionHandlerExtension
    {
        public static void ConfigureExceptionHandler<T>(this WebApplication application,ILogger<T> logger) //extension fonksiyon.
        {
            application.UseExceptionHandler(builder =>
            { // herhangi bir noktada yasaacak patlamayı run fonksiyonu ile yakaladık.
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = MediaTypeNames.Application.Json;// "application/json" demek;
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        logger.LogError(contextFeature.Error.Message);

                   await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCodeContext = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                        Tittle = "Hata alındı"
                    });

                    }

                });
            });
        }
    }
}
