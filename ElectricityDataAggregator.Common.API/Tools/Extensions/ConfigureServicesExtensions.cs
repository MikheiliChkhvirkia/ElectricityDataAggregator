using ElectricityDataAggregator.Common.API.Swagger;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ElectricityDataAggregator.Common.API.Tools.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static void AddCommonApi(this WebApplicationBuilder builder, Assembly assembly)
        {
            builder.Services.AddApiProblemDetailsFactory()
                .AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHealthChecks(builder.Configuration, assembly);
            builder.Services.AddApiVersioningOptions();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwagger();

            builder.Services.ConfigureControllers();
        }

        #region Private
        private static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            IHealthChecksBuilder builder = services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
            string connectionString = configuration.GetConnectionString("SQL");

        }
        private static void AddApiVersioningOptions(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
            });
        }
        private static void AddSwagger(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfiguration>();

            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();

                options.UseInlineDefinitionsForEnums();

                options.CustomSchemaIds(type => type.ToString());
            });
        }
        private static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                config.CacheProfiles.Add("1800SecondsCaching", new CacheProfile
                {
                    Duration = 1800
                });
            });
        }
        #endregion
    }
}
