using TodoWeb.Models;

namespace TodoWeb.TodoVM
{
    public class ProjectVM
    {
        public List<Projects> Projects { get; set; }
        public List<User> Users { get; set; }
        public Projects ProjectsAdd { get; set; }
        public string DueDate { get; set; }
    }
}
