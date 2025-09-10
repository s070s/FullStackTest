using System.ComponentModel.DataAnnotations;

namespace Api.Models.BaseClasses
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        [MaxLength(50)]
        public string? UpdatedBy { get; set; }
    }
}