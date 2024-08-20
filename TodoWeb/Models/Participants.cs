namespace TodoWeb.Models
{
    public class Participants
    {
        public int ProjectId { get; set; }
        public Projects Projects { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
