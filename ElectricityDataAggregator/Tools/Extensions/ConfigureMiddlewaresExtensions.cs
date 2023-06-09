using ElectricityDataAggregator.API.Middleware;
using ElectricityDataAggregator.Common.API.ErrorHandling;
using ElectricityDataAggregator.Common.API.Tools.Extensions;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Extensions;
using System.Reflection;

namespace ElectricityDataAggregator.API.Tools.Extensions
{
    public static class ConfigureMiddlewaresExtensions
    {
        public static void ConfigureMiddlewares(this WebApplication app, Assembly assembly)
        {
            app.UseApiErrorHandling().AddExceptionHandler<ExceptionHandler>();
            app.UsePathBase();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<CurrentStateMiddleware>();
            app.UseHttpsRedirection();

            app.UseCors(options => options
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
            );

            app.UseEndpoints(app.Environment.IsProduction());
            app.UseSwagger(assembly);
        }
    }
}
