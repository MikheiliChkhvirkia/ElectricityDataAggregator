using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Builders;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Options;
using Serilog.Context;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Middlewares
{
    public class ErrorHandlingMiddleware<TApiProblemDetails>
        where TApiProblemDetails : ApiProblemDetails, new()
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware<TApiProblemDetails>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ErrorHandlingMiddleware(RequestDelegate next,
            ILogger<ErrorHandlingMiddleware<TApiProblemDetails>> logger,
            IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var ctx = ApiProblemDetailsBuilder.Current.BuildProblemDetails(context, exception, _serviceProvider);

            if (ctx.Logging.EnableLogging)
                LogError(ctx);

            //Send response
            var result = JsonSerializer.Serialize(ctx.ProblemDetails, new JsonSerializerOptions
            {
                IgnoreNullValues = true
            });
            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = ctx.ProblemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(result);
        }

        private void LogError(ApiProblemContext<Exception> ctx)
        {
            if (ErrorHandlingOptions.Current.CustomLogAction != null)
            {
                ErrorHandlingOptions.Current.CustomLogAction.Invoke(new ApiProblemLogContext(
                    ctx.ProblemDetails,
                    _logger,
                    ctx.HttpContext,
                    ctx.Exception,
                    _serviceProvider,
                    ctx.Logging.LogLevel
                    ));
            }
            else if (ErrorHandlingOptions.Current.LogAction != null)
            {
                ErrorHandlingOptions.Current.LogAction.Invoke(_logger, ctx.ProblemDetails, ctx.Exception);
            }
            else
            {
                _logger.Log(ctx.Logging.LogLevel, ctx.Exception,
                    "Api {LogLevel} occurred. TraceId: {TraceId}, Code: {Code}, Title: {Title}, Detail: {Detail}",
                    ctx.Logging.LogLevel, ctx.ProblemDetails.TraceId,
                    ctx.ProblemDetails.Code, ctx.ProblemDetails.Title, ctx.ProblemDetails.Detail);

                LogContext.PushProperty(Convert.ToString(ctx.Logging.LogLevel),
                    string.Format("\nApi {0} Occurred, \nTraceId: {1}, \nCode: {2}, \nTitle: {3}, \nDetail: {4}\n",
                    Convert.ToString(ctx.Logging.LogLevel), ctx.ProblemDetails.TraceId,
                    ctx.ProblemDetails.Code, ctx.ProblemDetails.Title, ctx.ProblemDetails.Detail));

                Serilog.Log.Information(ctx.Exception.Message);
            }
        }
    }
}
