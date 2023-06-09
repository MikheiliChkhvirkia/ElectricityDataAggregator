using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using ElectricityDataAggregator.Common.API.Tools.Constants;
using System.Reflection;

namespace ElectricityDataAggregator.Common.API.Tools.Extensions
{
    public static class ConfigureMiddlewaresExtensions
    {
        public static void UsePathBase(this IApplicationBuilder app)
        {
            if (!string.IsNullOrWhiteSpace(EnvironmentVariables.PathBase))
            {
                app.UsePathBase($"/{EnvironmentVariables.PathBase}");
            }
        }

        public static void UseEndpoints(this IApplicationBuilder app, bool isProduction)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = registration => registration.Name.Contains("self")
                });

                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                });

                endpoints.MapGet("/", context =>
                {
                    if (!isProduction)
                    {
                        context.Response.Redirect($"{(!string.IsNullOrWhiteSpace(EnvironmentVariables.PathBase) ? $"/{EnvironmentVariables.PathBase}" : string.Empty)}/swagger");

                        return Task.FromResult(0);
                    }

                    return context.Response.WriteAsync("OK");
                });
            });
        }

        public static void UseSwagger(this IApplicationBuilder app, Assembly assembly)
        {
            IApiVersionDescriptionProvider provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"{(!string.IsNullOrWhiteSpace(EnvironmentVariables.PathBase) ? $"/{EnvironmentVariables.PathBase}" : string.Empty)}/swagger/{description.GroupName}/swagger.json",
                        $"{assembly.GetName().Name} {description.GroupName}");
                }
            });
        }
    }
}
