using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Text.Json;

namespace BlazorEventsTodo
{
    public static class DefaultJsonOptions
    {
        public static readonly JsonSerializerOptions Options =
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
            .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }
}

