using System.Text.Json.Serialization;

namespace Wault.CS.Models
{
    public class ClaimModel
    {
        [JsonPropertyName("value")]
        public dynamic Value { get; set; }
    }
}
