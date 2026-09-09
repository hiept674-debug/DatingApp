namespace DatingApp.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
            = new List<Message>();
    }
}