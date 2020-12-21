using System;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace Wault.CS.Webhook
{
    public class WebhookPayload
    {
        [JsonPropertyName("action")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WebhookAction Action { get; set; }

        [JsonPropertyName("trackId")]
        public string TrackId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
