namespace ElectricityDataAggregator.Common.ApiErrorHandling.Models.ErrorCodes
{
    public static class ErrorCodeDescriptions
    {
        private static readonly Dictionary<string, ErrorCodeDescription> ErrorCodes = new();

        public static IEnumerable<ErrorCodeDescription> AllErrorCodes =>
            ErrorCodes.Select(x => x.Value);

        internal static void Add(ErrorCodeDescription code)
        {
            if (code == null)
                return;

            ErrorCodes[code.Code] = code;
        }

        internal static void AddRange(IEnumerable<ErrorCodeDescription> errorCodes)
        {
            if (errorCodes == null)
                return;

            foreach (var code in errorCodes)
            {
                ErrorCodes[code.Code] = code;
            }
        }
    }
}
