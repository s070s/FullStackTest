namespace Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedUtc { get; set; }
        public bool IsActive { get; set; }
    public string PasswordHash { get; set; } = null!;
    }
}
