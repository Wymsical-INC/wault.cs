﻿using System;
using System.Text.Json.Serialization;

namespace Wault.CS.Models
{
    public class DocumentCreateResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
