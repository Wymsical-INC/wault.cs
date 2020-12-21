using System.Text.Json.Serialization;

namespace Wault.CS.Models
{
    public class AccessTokenResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("input")]
        public object Input { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public dynamic Value { get; set; }
    }
}
