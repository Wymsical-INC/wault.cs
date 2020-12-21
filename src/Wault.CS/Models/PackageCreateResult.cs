using System;
using System.Text.Json.Serialization;

namespace Wault.CS.Models
{
    public class PackageCreateResult
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
    }
}
