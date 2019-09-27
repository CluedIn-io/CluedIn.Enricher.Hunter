using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Hunter.Models
{
    public class Meta
    {
        [JsonProperty("results")]
        public long Results { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("params")]
        public Params Params { get; set; }
    }
}
