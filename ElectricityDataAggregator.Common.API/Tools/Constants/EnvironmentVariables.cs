namespace ElectricityDataAggregator.Common.API.Tools.Constants
{
    public static class EnvironmentVariables
    {
        // Set request path base in case API is published under some path (e.g. virtual directory) instead of root path.
        // If API is published in IIS virtual directory, then ASPNETCORE_APPL_PATH env variable is automatically set.
        // If API is published behind reverse proxy, under some path, then ASPNETCORE_APPL_PATH env variable or appsettings must be set manually.
        public static readonly string PathBase = Environment.GetEnvironmentVariable("ASPNETCORE_APPL_PATH");
    }
}
