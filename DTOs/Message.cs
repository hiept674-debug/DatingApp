namespace DatingApp.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int SenderId { get; set; }

        public string SenderName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }
    }

    public class SendMessageDto
    {
        public int ConversationId { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}