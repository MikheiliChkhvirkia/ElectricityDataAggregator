using Newtonsoft.Json;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.JsonConverters.NetwonsoftJson
{
    internal class AnnotatedApiProblemDetails
    {
        /// <remarks>
        /// Required for JSON.NET deserialization.
        /// </remarks>
        public AnnotatedApiProblemDetails() { }

        public AnnotatedApiProblemDetails(ApiProblemDetails problemDetails)
        {
            Detail = problemDetails.Detail;
            Instance = problemDetails.Instance;
            Status = problemDetails.Status;
            Title = problemDetails.Title;
            Type = problemDetails.Type;
            Code = problemDetails.Code;
            TraceId = problemDetails.TraceId;

            foreach (var kvp in problemDetails.Extensions)
            {
                Extensions[kvp.Key] = kvp.Value;
            }

            if (problemDetails.Errors != null)
            {
                Errors ??= new Dictionary<string, string[]>(StringComparer.Ordinal);
                foreach (var kvp in problemDetails.Errors)
                {
                    Errors[kvp.Key] = kvp.Value;
                }
            }
        }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public int? Status { get; set; }

        [JsonProperty(PropertyName = "detail", NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }

        [JsonProperty(PropertyName = "instance", NullValueHandling = NullValueHandling.Ignore)]
        public string Instance { get; set; }

        [JsonProperty(PropertyName = "code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "traceId", NullValueHandling = NullValueHandling.Ignore)]
        public string TraceId { get; set; }

        [JsonProperty(PropertyName = "errors", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string[]> Errors { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

        public void CopyTo(ApiProblemDetails problemDetails)
        {
            problemDetails.Type = Type;
            problemDetails.Title = Title;
            problemDetails.Status = Status;
            problemDetails.Instance = Instance;
            problemDetails.Detail = Detail;
            problemDetails.Code = Code;
            problemDetails.TraceId = TraceId;

            foreach (var kvp in Extensions)
            {
                problemDetails.Extensions[kvp.Key] = kvp.Value;
            }

            if (Errors != null)
            {
                problemDetails.Errors ??= new Dictionary<string, string[]>(StringComparer.Ordinal);
                foreach (var kvp in Errors)
                {
                    problemDetails.Errors[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}
