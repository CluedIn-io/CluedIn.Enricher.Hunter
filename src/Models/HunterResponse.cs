using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CluedIn.ExternalSearch.Providers.Hunter.Models
{
    public class HunterResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
