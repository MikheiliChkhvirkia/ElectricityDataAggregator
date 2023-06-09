using Microsoft.Extensions.Logging;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Options
{
    public class ErrorHandlingOptions
    {
        private static ErrorHandlingOptions _options;

        public static ErrorHandlingOptions Current => _options ??= new ErrorHandlingOptions();

        private ErrorHandlingOptions() { }

        /// <summary>
        /// Log problem details
        /// <value>(default: true)</value>
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        /// Default logging severity level
        /// <value>(default: LogLevel.Error)</value>
        /// </summary>
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Error;

        [Obsolete("Please use CustomLogAction property")]
        public Action<ILogger, ApiProblemDetails, Exception> LogAction { get; set; }

        /// <summary>
        /// Override default log action
        /// </summary>
        public Action<ApiProblemLogContext> CustomLogAction { get; set; }

        public bool EnableErrorCodesEndpoint { get; set; } = true;
        public string ErrorCodesEndpointPath { get; set; } = "/error-codes";
    }
}