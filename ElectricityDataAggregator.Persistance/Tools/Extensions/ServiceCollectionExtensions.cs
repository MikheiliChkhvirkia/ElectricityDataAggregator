using ElectricityDataAggregator.Application.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ElectricityDataAggregator.Persistence.Tools.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IElectricityDbContext, ElectricityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQL"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    sqlOptions.EnableRetryOnFailure();
                }));

            var serviceProvider = services.BuildServiceProvider();
            IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();
            serviceProvider.GetRequiredService<IElectricityDbContext>().Database.Migrate();
        }
    }
}
