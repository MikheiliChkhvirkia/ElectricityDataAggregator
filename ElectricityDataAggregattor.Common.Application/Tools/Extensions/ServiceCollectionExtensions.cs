using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ElectricityDataAggregator.Common.Application.Tools.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonApplication(this IServiceCollection services, Assembly assembly)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(currentAssembly);
            services.AddMediatR(assembly);
        }
    }
}
