using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }

        public Projects ProjectId { get; set; }

        public User AssignedTo { get; set; }

        [Required]
        public string TaskTitle { get; set; }

        [Required]
        public string TaskDescription { get; set; }

        public bool IsCompleted { get; set; }

        public DateOnly DueDate { get; set; }

        public DateTime? CreatedOn { get; set; }

    }
}
