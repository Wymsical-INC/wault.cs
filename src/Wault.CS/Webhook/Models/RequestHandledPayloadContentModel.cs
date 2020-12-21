using System.Text.Json.Serialization;

namespace Wault.CS.Webhook.Models
{
    public class RequestHandledPayloadContentModel
    {
        [JsonPropertyName("waultId")]
        public string WaultId { get; set; }
    }
}
