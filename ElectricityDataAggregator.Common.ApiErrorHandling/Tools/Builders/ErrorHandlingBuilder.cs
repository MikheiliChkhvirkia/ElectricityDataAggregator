using ElectricityDataAggregator.Common.ApiErrorHandling.Contracts;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models.ErrorCodes;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Builders
{
    public class ErrorHandlingBuilder
    {
        private static ErrorHandlingBuilder _builder;

        internal static ErrorHandlingBuilder Current
        {
            get
            {
                if (_builder != null)
                    return _builder;

                _builder = new ErrorHandlingBuilder();
                _builder.AddErrorCodeDescriptions(DefaultErrorCodes.DefaultErrorCodesList);

                return _builder;
            }
        }

        public ErrorHandlingBuilder AddExceptionHandler<TExceptionHandler>()
            where TExceptionHandler : IExceptionHandler<Exception>
        {
            ApiProblemDetailsBuilder.Current.AddExceptionHandler<TExceptionHandler>();
            return this;
        }

        public ErrorHandlingBuilder AddErrorCodeDescriptions(List<ErrorCodeDescription> errorCodes)
        {
            if (errorCodes == null || errorCodes.Count == 0)
                return this;

            ErrorCodeDescriptions.AddRange(errorCodes);

            return this;
        }
    }
}
