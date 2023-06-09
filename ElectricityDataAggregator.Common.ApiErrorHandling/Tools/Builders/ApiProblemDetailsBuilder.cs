using ElectricityDataAggregator.Common.ApiErrorHandling.Contracts;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models.ErrorCodes;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Helpers;
using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ElectricityDataAggregator.Common.ApiErrorHandling.Tests")]
namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Builders
{
    public class ApiProblemDetailsBuilder
    {
        private static ApiProblemDetailsBuilder _apiProblemDetailsBuilder;

        private readonly Dictionary<Type, Action<ApiProblemContext<Exception>>> _builderActions;

        private readonly Dictionary<Type, object> _exceptionHandlers;

        internal static ApiProblemDetailsBuilder Current
        {
            get
            {
                if (_apiProblemDetailsBuilder != null)
                    return _apiProblemDetailsBuilder;

                _apiProblemDetailsBuilder = new ApiProblemDetailsBuilder();

                return _apiProblemDetailsBuilder;
            }
        }

        private ApiProblemDetailsBuilder()
        {
            _builderActions = new Dictionary<Type, Action<ApiProblemContext<Exception>>>();
            _exceptionHandlers = new Dictionary<Type, object>();

            When<Exception>(context => { });
        }

        public ApiProblemDetailsBuilder AddExceptionHandler<TExceptionHandler>()
            where TExceptionHandler : IExceptionHandler<Exception>
        {
            var errorHandlerType = typeof(TExceptionHandler);

            var methods = errorHandlerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name == nameof(IExceptionHandler<Exception>.Handle)
                    && x.GetParameters() is { } parameters
                    && parameters.Length == 1
                    && parameters.First().ParameterType is { } paramType
                    && paramType.IsGenericType
                    && paramType.GetGenericTypeDefinition() is { } gtd
                    && gtd == typeof(ApiProblemContext<>))
                .ToList();

            var errorHandlerObject = Activator.CreateInstance(errorHandlerType);

            foreach (var method in methods)
            {
                var exceptionType = method.GetParameters().First().ParameterType.GetGenericArguments()[0];
                var problemContextType = typeof(ApiProblemContext<>).MakeGenericType(exceptionType);

                _builderActions[exceptionType] = BuilderAction;
                _exceptionHandlers[exceptionType] = errorHandlerObject;

                void BuilderAction(ApiProblemContext<Exception> context)
                {
                    var errorHandler = _exceptionHandlers[exceptionType];
                    method.Invoke(errorHandler,
                        new object[]
                        {
                            Activator.CreateInstance(problemContextType, new object[]
                            {
                                context.ProblemDetails,
                                context.HttpContext,
                                context.Exception,
                                context.ServiceProvider,
                                context.Logging
                            })
                        });
                };
            }

            return this;
        }

        public ApiProblemDetailsBuilder When<TException>(Action<ApiProblemContext<TException>> builderAction)
            where TException : Exception
        {
            var exceptionType = typeof(TException);

            _builderActions[exceptionType] = BuilderAction;

            void BuilderAction(ApiProblemContext<Exception> context) => builderAction(new ApiProblemContext<TException>
            (
                context.ProblemDetails,
                context.HttpContext,
                (TException)context.Exception,
                context.ServiceProvider,
                context.Logging
            ));

            return this;
        }

        public ApiProblemContext<Exception> BuildProblemDetails(HttpContext context, Exception exception, IServiceProvider serviceProvider)
        {
            var problemDetails = new ApiProblemDetails();
            problemDetails.TraceId = Activity.Current?.Id ?? context?.TraceIdentifier ?? Guid.NewGuid().ToString();
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Code = DefaultErrorCodes.SystemError.Code;
            problemDetails.Title = DefaultErrorCodes.SystemError.Title;

            var problemContext = new ApiProblemContext<Exception>(problemDetails, context, exception, serviceProvider,
                new ApiProblemLogging(ErrorHandlingOptions.Current.EnableLogging,
                    ErrorHandlingOptions.Current.DefaultLogLevel));

            var builderAction = GetBuilderAction(exception.GetType());
            builderAction?.Invoke(problemContext);

            var apiBehaviorOptions = serviceProvider.GetService<IOptions<ApiBehaviorOptions>>();
            if (apiBehaviorOptions?.Value != null &&
                apiBehaviorOptions.Value.ClientErrorMapping.TryGetValue(problemDetails.Status.Value,
                    out var clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
            }

            problemDetails.Status ??= StatusCodes.Status500InternalServerError;
            problemDetails.Type ??= Helper.GetDefaultErrorType(problemDetails.Code,
                problemDetails.Status.Value);


            return problemContext;
        }

        private Action<ApiProblemContext<Exception>> GetBuilderAction(Type exceptionType)
        {
            if (_builderActions.ContainsKey(exceptionType))
            {
                return _builderActions[exceptionType];
            }

            if (exceptionType != typeof(Exception))
            {
                return GetBuilderAction(exceptionType.BaseType);
            }

            return null;
        }
    }
}
