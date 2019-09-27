using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Hunter.Models
{
    public class EmailCount
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("personal_emails")]
        public long PersonalEmails { get; set; }

        [JsonProperty("generic_emails")]
        public long GenericEmails { get; set; }

        [JsonProperty("department")]
        public Department Department { get; set; }

        [JsonProperty("seniority")]
        public Seniority Seniority { get; set; }
    }

    public class Department
    {
        [JsonProperty("executive")]
        public long Executive { get; set; }

        [JsonProperty("it")]
        public long It { get; set; }

        [JsonProperty("finance")]
        public long Finance { get; set; }

        [JsonProperty("management")]
        public long Management { get; set; }

        [JsonProperty("sales")]
        public long Sales { get; set; }

        [JsonProperty("legal")]
        public long Legal { get; set; }

        [JsonProperty("support")]
        public long Support { get; set; }

        [JsonProperty("hr")]
        public long Hr { get; set; }

        [JsonProperty("marketing")]
        public long Marketing { get; set; }

        [JsonProperty("communication")]
        public long Communication { get; set; }
    }

    public class Seniority
    {
        [JsonProperty("junior")]
        public long Junior { get; set; }

        [JsonProperty("senior")]
        public long Senior { get; set; }

        [JsonProperty("executive")]
        public long Executive { get; set; }
    }
}
