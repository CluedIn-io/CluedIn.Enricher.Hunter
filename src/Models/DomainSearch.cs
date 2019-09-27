using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Hunter.Models
{
    public class DomainSearch
    {
        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("disposable")]
        public bool Disposable { get; set; }

        [JsonProperty("webmail")]
        public bool Webmail { get; set; }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("emails")]
        public List<Email> Emails { get; set; }
    }
}
