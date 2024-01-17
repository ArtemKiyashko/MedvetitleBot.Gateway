namespace MedvetitleBot.Gateway.Models
{
    public class ChatEntity : BaseEntity
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool Enabled { get; set; } = true;
    }
}

