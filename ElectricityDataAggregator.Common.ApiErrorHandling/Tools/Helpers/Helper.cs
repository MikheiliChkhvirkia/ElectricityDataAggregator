using ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Options;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.Helpers
{
    internal static class Helper
    {
        internal static string GetDefaultErrorType(string errorCode, int statusCode)
        {
            return ErrorHandlingOptions.Current.EnableErrorCodesEndpoint
                ? $"{ErrorHandlingOptions.Current.ErrorCodesEndpointPath}#{errorCode}"
                : $"https://httpstatuses.com/{statusCode}";
        }
    }
}
