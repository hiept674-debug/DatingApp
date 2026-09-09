namespace DatingApp.DTOs
{
    public class ConversationDto
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public int OtherUserId { get; set; }

        public string OtherUserName { get; set; } = string.Empty;

        public string? OtherUserAvatar { get; set; }

        public string? LastMessage { get; set; }

        public DateTime? LastMessageAt { get; set; }
    }
}