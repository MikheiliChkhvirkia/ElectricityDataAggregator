using Microsoft.AspNetCore.Http;
using ElectricityDataAggregator.Common.API.Tools.Constants;

namespace ElectricityDataAggregator.Common.API.Tools.Extensions
{
    public static class PathStringExtensions
    {
        public static bool IsHealthCheckPath(this PathString pathString)
        {
            return pathString.Value == "/liveness" || pathString.Value == "/hc";
        }
        public static bool IsAllowedPath(this PathString pathString)
        {
            List<string> allowedPaths = new List<string> { "", "/" };


            return allowedPaths.Contains(pathString.Value) ||
                allowedPaths.Select(allowedPath => $"{allowedPath}{allowedPath}").Contains(pathString.Value) ||
                pathString.Value.StartsWith("/swagger") ||
                pathString.Value.StartsWith($"{EnvironmentVariables.PathBase}/swagger");
        }
    }
}
