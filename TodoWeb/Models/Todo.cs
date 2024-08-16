using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Models
{
    public class Todo
    {
        [Key]
        public int Note_id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        public bool IsCompleted { get; set; }

        public Category Category { get; set; }

        public User User { get; set; }
    }
}
