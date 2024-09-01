namespace TodoWeb.Models
{
    public class Message
    {
        public Guid ChatId { get; set; }
        public string IdUser { get; set; }
        public User User { get; set; }

        public int IdProject { get; set; }
        public Projects Projects { get; set; }

        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
