using Api.Models.Enums;
namespace Api.Models
{
    public class User: BaseEntity
    {
        public UserRole Role { get; set; } = UserRole.Client;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string? ProfilePhotoUrl { get; set; }

        public Trainer? TrainerProfile { get; set; }
        public Client? ClientProfile { get; set; }
    }
}
