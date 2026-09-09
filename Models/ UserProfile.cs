namespace DatingApp.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public string? Job { get; set; }

        public string? Hobbies { get; set; }

        // User
        public User User { get; set; } = null!;
    }
}