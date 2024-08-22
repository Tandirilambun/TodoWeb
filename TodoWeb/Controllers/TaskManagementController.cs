using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Data;
using TodoWeb.Models;
using TodoWeb.TodoVM;

namespace TodoWeb.Controllers
{
    //[Route("Project/{projectId}/Task")]
    public class TaskManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<User> _userManager;

        public TaskManagementController(UserManager<User> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }

        //[Route("Project/{projectId}/Task")]
        public async Task<IActionResult> Index(int projectId, string? status)
        {
            var projectQuery = await _db.Projects
                .Where(part => part.Participants.Any(proj => proj.ProjectId == projectId))
                .Include(part => part.Participants)
                .ThenInclude(user => user.User)
                .FirstOrDefaultAsync();
            var taskQuery = await _db.Tasks
                .Where(task => task.ProjectId.ProjectId == projectId)
                .ToListAsync();

            List<Tasks> taskRunning = new List<Tasks>();
            List<Tasks> taskComplete = new List<Tasks>();

            foreach (var taskItem in taskQuery) {
                if (taskItem.IsCompleted) { 
                    taskComplete.Add(taskItem);
                }
                else
                {
                    taskRunning.Add(taskItem);
                }
            }

            if (status == "running")
            {
                taskQuery = await _db.Tasks
                .Where(task => task.ProjectId.ProjectId == projectId)
                .Where(task => task.IsCompleted == false)
                .ToListAsync();
            }
            else if (status == "complete")
            {
                taskQuery = await _db.Tasks
                .Where(task => task.ProjectId.ProjectId == projectId)
                .Where(task => task.IsCompleted == true)
                .ToListAsync();
            }

            var viewModel = new TaskVM
            {
                Projects = projectQuery,
                Tasks = taskQuery,
                TasksComplete = taskComplete,
                TasksRunning = taskRunning
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CreateTask(TaskVM obj)
        {
            var projectQuery = await _db.Projects.FindAsync(obj.ProjectId);
            var userQuery = await _db.Users.FindAsync(obj.AssignTo);
            var taskDueDate = DateOnly.Parse(obj.DueDate);
            var task = new Tasks
            {
                ProjectId = projectQuery,
                AssignedTo = userQuery,
                TaskTitle = obj.TaskTitle,
                TaskDescription = obj.TaskDescription,
                IsCompleted = false,
                DueDate = taskDueDate,
                CreatedOn = DateTime.UtcNow
            };

            await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();

            TempData["success"] = "Task Created Successfully";
            return Redirect($"/Project/{obj.ProjectId}/Task");
        }

        public async Task<IActionResult> UpdateTask(TaskVM obj)
        {
            var taskQuery = await _db.Tasks.FindAsync(obj.TaskId);
            var projectQuery = await _db.Projects.FindAsync(obj.ProjectId);
            var userQuery = await _db.Users.FindAsync(obj.AssignTo);
            var taskDueDate = DateOnly.Parse(obj.DueDate);

            if (taskQuery != null) 
            {
                taskQuery.TaskId = obj.TaskId;
                taskQuery.TaskTitle = obj.TaskTitle;
                taskQuery.TaskDescription = obj.TaskDescription;
                taskQuery.IsCompleted = false;
                taskQuery.DueDate = taskDueDate;
                taskQuery.AssignedTo = userQuery;

                await _db.SaveChangesAsync();
                TempData["success"] = "Task Update Successfully";
                return Redirect($"/Project/{obj.ProjectId}/Task");
            }

            TempData["success"] = "Task Update Failed";
            return Redirect($"/Project/{obj.ProjectId}/Task");
        }

        public async Task<IActionResult> DoneTask(int task, int project)
        {
            var taskQuery = await _db.Tasks.FindAsync(task);

            if (taskQuery == null) { 
                return NotFound();
            }

            taskQuery.IsCompleted = true;
            await _db.SaveChangesAsync();
            TempData["success"] = "Task Update Successfully";

            return Redirect($"/Project/{project}/Task");
        }

        [HttpGet]
        [Route("GetData")]
        public async Task<IActionResult> GetData(int id)
        {
            try
            {
                //var taskQuery = await _db.Tasks.FindAsync(id);
                var taskQuery = await _db.Tasks
                    .Where(t => t.TaskId == id)
                    .Select(task => new
                    {
                        task.TaskId,
                        task.TaskTitle,
                        task.TaskDescription,
                        DueDate = task.DueDate.ToString("yyyy-MM-dd")
                    }).FirstOrDefaultAsync();

                return Json(taskQuery);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
