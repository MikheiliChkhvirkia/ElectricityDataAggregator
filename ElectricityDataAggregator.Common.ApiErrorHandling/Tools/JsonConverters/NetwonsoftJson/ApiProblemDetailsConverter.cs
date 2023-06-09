using Newtonsoft.Json;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.JsonConverters.NetwonsoftJson
{
    public sealed class ApiProblemDetailsConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ApiProblemDetails);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var annotatedProblemDetails = serializer.Deserialize<AnnotatedApiProblemDetails>(reader);
            if (annotatedProblemDetails == null)
            {
                return null;
            }

            var problemDetails = (ApiProblemDetails)existingValue ?? new ApiProblemDetails();
            annotatedProblemDetails.CopyTo(problemDetails);

            return problemDetails;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var problemDetails = (ApiProblemDetails)value;
            var annotatedProblemDetails = new AnnotatedApiProblemDetails(problemDetails);

            serializer.Serialize(writer, annotatedProblemDetails);
        }
    }
}
