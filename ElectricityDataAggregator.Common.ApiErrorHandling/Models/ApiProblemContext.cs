using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Models
{
    public class ApiProblemContext<TException>
        where TException : Exception
    {
        public ApiProblemDetails ProblemDetails { get; internal set; }
        public HttpContext HttpContext { get; internal set; }
        public TException Exception { get; internal set; }
        public IServiceProvider ServiceProvider { get; internal set; }
        public ApiProblemLogging Logging { get; internal set; }


        public ApiProblemContext() { }

        public ApiProblemContext(ApiProblemDetails problemDetails,
            HttpContext httpContext,
            TException exception,
            IServiceProvider serviceProvider,
            ApiProblemLogging logging)
        {
            ProblemDetails = problemDetails;
            HttpContext = httpContext;
            Exception = exception;
            ServiceProvider = serviceProvider;
            Logging = logging;
        }
    }

    public class ApiProblemLogging
    {
        public bool EnableLogging { get; set; }
        public LogLevel LogLevel { get; set; }

        public ApiProblemLogging(bool enableLogging, LogLevel logLevel)
        {
            EnableLogging = enableLogging;
            LogLevel = logLevel;
        }
    }
}
