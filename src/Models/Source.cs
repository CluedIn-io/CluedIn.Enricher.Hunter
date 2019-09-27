using System;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Hunter.Models
{
    public class Source
    {
        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("extracted_on")]
        public string ExtractedOn { get; set; }

        [JsonProperty("last_seen_on")]
        public string LastSeenOn { get; set; }

        [JsonProperty("still_on_page")]
        public bool StillOnPage { get; set; }
    }
}
