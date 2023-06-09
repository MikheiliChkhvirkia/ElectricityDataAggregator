using ElectricityDataAggregator.Application.Tools.Extensions;
using ElectricityDataAggregator.Common.API.Tools.Extensions;
using ElectricityDataAggregator.Persistence.Tools.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;
using System.Text;

namespace ElectricityDataAggregator.API.Tools.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder, Assembly assembly, ILoggerFactory loggerFactory)
        {
            builder.AddCommonApi(assembly);
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddMemoryCache();
            AddLogging(loggerFactory, builder.Configuration);
        }
        #region Private 
        private static void AddLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext().CreateLogger();
            loggerFactory.AddSerilog();
        }
        #endregion
    }
}
