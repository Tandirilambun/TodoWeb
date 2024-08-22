using TodoWeb.Models;

namespace TodoWeb.TodoVM
{
    public class TaskVM
    {
        public Projects Projects { get; set; }
        public List<Tasks> Tasks {get; set;}
        public List<Tasks> TasksComplete {get; set;}
        public List<Tasks> TasksRunning {get; set;}
        public List<User> Users { get; set; }
        public int ProjectId { get; set; }

        public int TaskId { get; set; }
        public string TaskTitle {  get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
        public string DueDate { get; set; }
        public string AssignTo { get; set; }
    }
}
