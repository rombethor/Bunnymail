using System.Text.Json.Serialization;

namespace Bunnymail.Models
{
    public class MailOutMessage
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;

        [JsonPropertyName("recipient")]
        public string Recipient { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public Dictionary<string, string>? Data { get; set; } = new Dictionary<string, string>();
    }
}
