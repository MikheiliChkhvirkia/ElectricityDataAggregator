using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models.ErrorCodes;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Options;
using System.Diagnostics;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Factories
{
    public class ApiProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions _options;

        private readonly string _defaultErrorCode;

        public ApiProblemDetailsFactory(IOptions<ApiBehaviorOptions> options,
            string defaultErrorCode = null,
            string validationErrorCode = null)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _defaultErrorCode = defaultErrorCode ?? DefaultErrorCodes.SystemError.Code;
        }

        public override ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            statusCode ??= StatusCodes.Status500InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            problemDetails.Extensions["code"] = _defaultErrorCode;

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= StatusCodes.Status400BadRequest;

            var problemDetails = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = statusCode,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            if (title != null)
            {
                // For validation problem details, don't overwrite the default title with null.
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        public ApiProblemDetails CreateApiValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string title = null,
            string detail = null,
            string instance = null,
            string type = null)
        {
            var valProblems = new ValidationProblemDetails(modelStateDictionary);

            var problemDetails = new ApiProblemDetails
            {
                Status = statusCode ?? StatusCodes.Status400BadRequest,
                Title = title ?? valProblems.Title,
                Detail = detail,
                Instance = instance,
                Type = type,
                Errors = valProblems.Errors
            };

            ApplyApiProblemDetailsDefaults(httpContext, problemDetails);

            return problemDetails;
        }

        private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {
            problemDetails.Status ??= statusCode;

            if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
            }

            problemDetails.Type ??= ErrorHandlingOptions.Current.EnableErrorCodesEndpoint
                ? $"{ErrorHandlingOptions.Current.ErrorCodesEndpointPath}#{problemDetails.Extensions["code"]}"
                : $"https://httpstatuses.com/{problemDetails.Status}";

            problemDetails.Title ??= DefaultErrorCodes.SystemError.Title;

            problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        }

        private void ApplyApiProblemDetailsDefaults(HttpContext httpContext, ApiProblemDetails apiProblemDetails)
        {
            if (_options.ClientErrorMapping.TryGetValue(apiProblemDetails.Status!.Value, out var clientErrorData))
            {
                apiProblemDetails.Title ??= clientErrorData.Title;
            }
            apiProblemDetails.Type ??= ErrorHandlingOptions.Current.EnableErrorCodesEndpoint
                ? $"{ErrorHandlingOptions.Current.ErrorCodesEndpointPath}#{apiProblemDetails.Code}"
                : $"https://httpstatuses.com/{apiProblemDetails.Status}";

            apiProblemDetails.Title ??= DefaultErrorCodes.SystemError.Title;
            apiProblemDetails.TraceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        }
    }
}
