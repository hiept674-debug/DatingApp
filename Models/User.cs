namespace DatingApp.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Quan hệ 1 - 1 với UserProfile
        public UserProfile? Profile { get; set; }

        // Tin nhắn đã gửi
        public ICollection<Message> Messages { get; set; }
            = new List<Message>();

        // Thông báo
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}