using Microsoft.AspNetCore.Builder;
using ElectricityDataAggregator.Common.ApiErrorHandling.Middlewares;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Builders;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Options;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static ErrorHandlingBuilder UseApiErrorHandling(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseApiErrorHandling<ApiProblemDetails>(null);
        }

        public static ErrorHandlingBuilder UseApiErrorHandling<TApiProblemDetails>(this IApplicationBuilder applicationBuilder,
            Action<ErrorHandlingOptions> options)
            where TApiProblemDetails : ApiProblemDetails, new()
        {
            options?.Invoke(ErrorHandlingOptions.Current);

            applicationBuilder.UseMiddleware<ErrorHandlingMiddleware<TApiProblemDetails>>();

            return ErrorHandlingBuilder.Current;
        }
    }
}
