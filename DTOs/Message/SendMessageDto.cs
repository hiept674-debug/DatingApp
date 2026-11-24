namespace DatingAppAPI.DTOs.Message;

public class SendMessageDto
{
    public int ConversationId { get; set; }

    public string Content { get; set; } = string.Empty;
}