using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Hunter.Models
{
    public class Params
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("full_name")]
        public object FullName { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("type")]
        public object Type { get; set; }

        [JsonProperty("seniority")]
        public object Seniority { get; set; }

        [JsonProperty("department")]
        public object Department { get; set; }
    }
}
