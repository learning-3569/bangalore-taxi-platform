using BangaloreTaxi.Api.Application;
using BangaloreTaxi.Api.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;

namespace BangaloreTaxi.Api;

public static class WebApplicationPipeline
{
    public static WebApplication UseApiFoundation(this WebApplication app)
    {
        app.UseForwardedHeaders();
        app.UseExceptionHandler();
        app.UseStatusCodePages(async statusCodeContext =>
        {
            var httpContext = statusCodeContext.HttpContext;
            if (httpContext.Response.HasStarted)
            {
                return;
            }

            var problemDetails = httpContext.RequestServices.GetService<IProblemDetailsService>();
            if (problemDetails is not null)
            {
                await problemDetails.WriteAsync(new ProblemDetailsContext { HttpContext = httpContext });
            }
        });
        app.UseMiddleware<SecurityHeadersMiddleware>();

        app.Use(async (context, next) =>
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("BangaloreTaxi.Api.Http");
            logger.LogInformation(
                "HTTP {Method} {Path} started {TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                context.TraceIdentifier);
            await next();
            logger.LogInformation(
                "HTTP {Method} {Path} {StatusCode} {TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                context.TraceIdentifier);
        });

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(ServiceCollectionExtensions.CorsPolicyName);
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health/live", HealthResponseWriter.LiveOptions())
            .DisableRateLimiting();
        app.MapHealthChecks("/health/ready", HealthResponseWriter.ReadyOptions())
            .DisableRateLimiting();
        app.MapHealthChecks("/health", HealthResponseWriter.ReadyOptions())
            .DisableRateLimiting();

        if (app.Environment.IsEnvironment("Testing"))
        {
            app.MapGet("/api/v1/_test/error", (HttpContext _) => throw new InvalidOperationException("Synthetic failure"))
                .ExcludeFromDescription();
            app.MapGet("/api/v1/_test/conflict", (HttpContext _) => throw new ConflictException("Assignment window overlaps."))
                .ExcludeFromDescription();
        }

        app.MapControllers();
        return app;
    }
}
