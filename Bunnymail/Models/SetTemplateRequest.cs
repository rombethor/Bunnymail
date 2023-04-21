using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bunnymail.Models
{
    public class TemplateOptions
    {
        [Required, JsonPropertyName("templateId")]
        public string? TemplateId { get; set; }

        [Required, JsonPropertyName("fromName")]
        public string? FromName { get; set; }

        [Required, JsonPropertyName("fromAddress")]
        public string? FromAddress { get; set; }
    }
}
