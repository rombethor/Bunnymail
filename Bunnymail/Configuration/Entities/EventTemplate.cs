using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bunnymail.Entities
{
    [Table(nameof(EventTemplate))]
    public class EventTemplate
    {
        [Key, Required, MaxLength(36)]
        public string? Event { get; set; }

        [Required, MaxLength(255)]
        public string? TemplateID { get; set; }

        [Required, MaxLength(36)]
        public string? FromName { get; set; }

        [Required, MaxLength(255)]
        public string? FromAddress { get; set; }
    }
}
