using Api.Models.BaseClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    public class RefreshToken : BaseEntity
    {
        [Required]
    [MaxLength(128)]
        public string TokenHash { get; set; } = null!;

    [MaxLength(128)]
        public string? ReplacedByTokenHash { get; set; }

        [Required]
        public DateTime ExpiresUtc { get; set; }

        public DateTime? RevokedUtc { get; set; }

        [MaxLength(45)]
        public string? CreatedByIp { get; set; }

        [MaxLength(45)]
        public string? RevokedByIp { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresUtc;

        [NotMapped]
        public bool IsActive => RevokedUtc is null && !IsExpired;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
