using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Models
{
    public class ApiProblemLogContext
    {
        public ApiProblemDetails ProblemDetails { get; internal set; }
        public ILogger Logger { get; internal set; }
        public HttpContext HttpContext { get; internal set; }
        public Exception Exception { get; internal set; }
        public IServiceProvider ServiceProvider { get; internal set; }

        public LogLevel LogLevel { get; set; }

        public ApiProblemLogContext() { }

        public ApiProblemLogContext(ApiProblemDetails problemDetails, ILogger logger, HttpContext httpContext, Exception exception, IServiceProvider serviceProvider, LogLevel logLevel)
        {
            ProblemDetails = problemDetails;
            Logger = logger;
            HttpContext = httpContext;
            Exception = exception;
            ServiceProvider = serviceProvider;
            LogLevel = logLevel;
        }
    }
}