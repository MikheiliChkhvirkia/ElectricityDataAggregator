using ElectricityDataAggregator.Common.ApiErrorHandling.Contracts;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models.ErrorCodes;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.JsonConverters.NetwonsoftJson;
using ElectricityDataAggregator.Common.Exceptions;
using ElectricityDataAggregator.Common.Tools.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestEase;
using System.Collections;

namespace ElectricityDataAggregator.Common.API.ErrorHandling
{
    public class ExceptionHandler : IExceptionHandler<Exception>,
         IExceptionHandler<AppException>,
         IExceptionHandler<HttpException>,
         IExceptionHandler<ObjectNotFoundException>,
         IExceptionHandler<FileIsNotFoundException>
    {
        private static readonly ApiProblemDetailsConverter apiProblemDetailsConverter = new();
        public void Handle(ApiProblemContext<Exception> ctx)
        {
            ctx.ProblemDetails.Type = "AppException";
            ctx.ProblemDetails.Title = ctx.Exception.Message;
            ctx.ProblemDetails.Status = StatusCodes.Status500InternalServerError;
            ctx.ProblemDetails.Detail = ctx.Exception.Message;

            foreach (DictionaryEntry item in ctx.Exception.Data)
            {
                ctx.ProblemDetails.Extensions.Add(new KeyValuePair<string, object>(item.Key.ToString().ToCamelCase(), item.Value));
            }

            ctx.ProblemDetails.Code = "AppException";
            ctx.Logging.LogLevel = LogLevel.Error;
        }

        public void Handle(ApiProblemContext<AppException> ctx)
        {
            ctx.ProblemDetails.Type = "AppException";
            ctx.ProblemDetails.Title = ctx.Exception.Title;
            ctx.ProblemDetails.Status = StatusCodes.Status400BadRequest;
            ctx.ProblemDetails.Detail = ctx.Exception.Message;
            ctx.ProblemDetails.Code = ctx.Exception.Code;
            ctx.Logging.LogLevel = LogLevel.Error;
        }

        public void Handle(ApiProblemContext<HttpException> ctx)
        {
            ctx.ProblemDetails.Type = "HttpException";
            ctx.ProblemDetails.Title = ctx.Exception.Title;
            ctx.ProblemDetails.Status = StatusCodes.Status502BadGateway;
            ctx.ProblemDetails.Detail = ctx.Exception.Message;
            ctx.ProblemDetails.Code = ctx.Exception.Code;
            ctx.ProblemDetails.Extensions["endpoint"] = ctx.Exception.Endpoint;
            ctx.Logging.LogLevel = LogLevel.Error;
        }

        public void Handle(ApiProblemContext<ObjectNotFoundException> ctx)
        {
            ctx.ProblemDetails.Type = "ObjectNotFoundException";
            ctx.ProblemDetails.Title = ctx.Exception.Title;
            ctx.ProblemDetails.Status = StatusCodes.Status404NotFound;
            ctx.ProblemDetails.Detail = ctx.Exception.Message;
            ctx.ProblemDetails.Code = ctx.Exception.Code;
            ctx.Logging.LogLevel = LogLevel.Error;
        }

        public void Handle(ApiProblemContext<FileIsNotFoundException> ctx)
        {
            ctx.ProblemDetails.Type = "FileNotFoundException";
            ctx.ProblemDetails.Title = ctx.Exception.Title;
            ctx.ProblemDetails.Status = StatusCodes.Status404NotFound;
            ctx.ProblemDetails.Detail = ctx.Exception.Message;
            ctx.ProblemDetails.Code = ctx.Exception.Code;
            ctx.Logging.LogLevel = LogLevel.Error;
        }

        public void Handle(ApiProblemContext<ApiException> ctx)
        {
            ApiProblemDetails problemDetails = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(ctx.Exception.Content))
                {
                    problemDetails = JsonConvert.DeserializeObject<ApiProblemDetails>(ctx.Exception.Content!, apiProblemDetailsConverter);
                }
            }
            catch { }

            if (problemDetails != null)
            {
                ctx.ProblemDetails.Type = problemDetails.Type;
                ctx.ProblemDetails.Title = problemDetails.Title;
                ctx.ProblemDetails.Status = problemDetails.Status;
                ctx.ProblemDetails.Detail = problemDetails.Detail;
                ctx.ProblemDetails.Code = problemDetails.Code;
                ctx.Logging.LogLevel = LogLevel.Error;
            }
            else
            {
                ctx.ProblemDetails.Type = "HttpException";
                ctx.ProblemDetails.Title = DefaultErrorCodes.SystemError.Title;
                ctx.ProblemDetails.Status = StatusCodes.Status500InternalServerError;
                ctx.ProblemDetails.Detail = DefaultErrorCodes.SystemError.Description;
                ctx.ProblemDetails.Code = "HttpException";
                ctx.Logging.LogLevel = LogLevel.Error;
                ctx.Logging.EnableLogging = false;
                ctx.ServiceProvider.GetRequiredService<ILogger<ExceptionHandler>>().LogError(ctx.Exception,
                    "Error detail, request URL: {RequestUrl}, ResponseBody: {ResponseBody}",
                    ctx.Exception.RequestUri?.ToString(),
                    ctx.Exception.Content);
            }
        }
    }
}
