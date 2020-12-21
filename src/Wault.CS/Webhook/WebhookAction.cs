using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Wault.CS.Webhook
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WebhookAction
    {
        [EnumMember(Value = "EntryRequestAccepted")]
        EntryRequestAccepted,

        [EnumMember(Value = "EntryRequestRejected")]
        EntryRequestRejected,

        [EnumMember(Value = "DocumentActivated")]
        DocumentActivated
    }
}
