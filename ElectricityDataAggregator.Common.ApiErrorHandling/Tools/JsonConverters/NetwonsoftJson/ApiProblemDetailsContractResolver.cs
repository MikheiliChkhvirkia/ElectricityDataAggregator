using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ElectricityDataAggregator.Common.ApiErrorHandling.Models;
using System.Reflection;

namespace ElectricityDataAggregator.Common.ApiErrorHandling.Tools.JsonConverters.NetwonsoftJson
{
    public class ApiProblemDetailsContractResolver : DefaultContractResolver
    {
        public ApiProblemDetailsContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }

        protected override JsonProperty CreateProperty(MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(ApiProblemDetails)
                && property.PropertyName != null
                && property.PropertyName.Equals("Extensions", StringComparison.OrdinalIgnoreCase))
            {
                property.Ignored = true;
            }

            return property;
        }
    }
}
