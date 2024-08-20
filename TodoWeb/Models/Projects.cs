using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Models
{
    public class Projects
    {
        [Key]
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateOnly DueDate { get; set; }

        public User CreatedBy { get; set; }

        public ICollection<Participants> Participants { get; set; }
    }
}
