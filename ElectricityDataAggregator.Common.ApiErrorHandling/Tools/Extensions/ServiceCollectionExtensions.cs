using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddApiProblemDetailsFactory(
            this IServiceCollection services,
            string defaultErrorCode = null,
            string validationErrorCode = null)
        {
            return services.AddSingleton<ProblemDetailsFactory>(serviceProvider =>
                new ApiProblemDetailsFactory(
                    serviceProvider.GetService<IOptions<ApiBehaviorOptions>>(),
                    defaultErrorCode,
                    validationErrorCode)
                );
        }
    }
}
