using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Wault.CS.Models
{
    public class EntryRequestResult
    {
        public EntryRequestResult()
        {
            AccessTokens = new List<AccessTokenResult>();
        }

        [JsonPropertyName("accessTokens")]
        public List<AccessTokenResult> AccessTokens { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("trackId")]
        public string TrackId { get; set; }
    }
}
