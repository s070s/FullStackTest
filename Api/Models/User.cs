using Api.Models.BaseClasses;
using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class User : BaseEntity
    {
        [Required]
        public UserRole Role { get; set; } = UserRole.Client;

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public bool IsActive { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; } = null!;

        [Url]
        public string? ProfilePhotoUrl { get; set; }

        public Trainer? TrainerProfile { get; set; }
        public Client? ClientProfile { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        
        
    }
}
