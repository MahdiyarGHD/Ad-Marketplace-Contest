using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Diagnostics;

namespace AdMarketplace.Extensions;

/// <summary>
/// extensions for global exception handling
/// </summary>
public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(
            errApp =>
            {
                errApp.Run(
                    async ctx =>
                    {
                        var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();

                        if (exHandlerFeature is not null)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            ctx.Response.ContentType = "application/problem+json";
                            
                            var result = Error.Unexpected(exHandlerFeature.Error.Message);

                            await ctx.Response.WriteAsJsonAsync(result);
                        }
                    });
            });

        return app;
    }
}