using ElectricityDataAggregator.Common.API.Tools.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace ElectricityDataAggregator.API.Middleware
{
    public class CurrentStateMiddleware
    {
        private readonly RequestDelegate next;

        public CurrentStateMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IMemoryCache cache)
        {
            if (!context.Request.Path.IsHealthCheckPath() && !context.Request.Path.IsAllowedPath())
            {
                //ToDo: Implement Logic
            }

            await next(context);
        }
    }
}