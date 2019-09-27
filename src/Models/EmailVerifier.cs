using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Hunter.Models
{
    public class EmailVerifier
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("regexp")]
        public bool Regexp { get; set; }

        [JsonProperty("gibberish")]
        public bool Gibberish { get; set; }

        [JsonProperty("disposable")]
        public bool Disposable { get; set; }

        [JsonProperty("webmail")]
        public bool Webmail { get; set; }

        [JsonProperty("mx_records")]
        public bool MxRecords { get; set; }

        [JsonProperty("smtp_server")]
        public bool SmtpServer { get; set; }

        [JsonProperty("smtp_check")]
        public bool SmtpCheck { get; set; }

        [JsonProperty("accept_all")]
        public bool AcceptAll { get; set; }

        [JsonProperty("block")]
        public bool Block { get; set; }

        [JsonProperty("sources")]
        public List<Source> Sources { get; set; }
    }
}
