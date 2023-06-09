namespace ElectricityDataAggregator.Common.ApiErrorHandling.Models.ErrorCodes
{
    public static class DefaultErrorCodes
    {
        public static ErrorCodeDescription SystemError = new(nameof(SystemError),
            "An error occurred while processing your request");

        public static List<ErrorCodeDescription> DefaultErrorCodesList => new()
        {
            SystemError
        };
    }
}
