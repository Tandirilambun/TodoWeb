using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Rotativa.AspNetCore;
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
            ViewBag.TaskCount = taskQuery.Count();

            var messageQuery = await _db.Messages
                .Where(msg => msg.IdProject == projectId)
                .Include(user => user.User)
                .ToListAsync();
            var taskRunning = taskQuery.Where(task => !task.IsCompleted).ToList();
            var taskComplete = taskQuery.Where(task => task.IsCompleted).ToList();

            // Filter task list based on status
            var filteredTasks = status switch
            {
                "running" => taskRunning,
                "complete" => taskComplete,
                _ => taskQuery
            };

            var viewModel = new TaskVM
            {
                Projects = projectQuery,
                Tasks = filteredTasks,
                TasksComplete = taskComplete,
                TasksRunning = taskRunning,
                Messages = messageQuery
                
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

        public async Task<IActionResult> ViewPdf(int id)
        {
            var taskQuery = await _db.Tasks
                .Where(t => t.ProjectId.ProjectId == id)
                .Include(user => user.AssignedTo)
                .ToListAsync();
            var projectQuery = await _db.Projects
                .FindAsync(id);
            var viewModel = new TaskVM
            {
                Projects = projectQuery,
                Tasks = taskQuery,
            };
            return new ViewAsPdf("ViewPdf", viewModel) { 
                FileName = $"Report_{projectQuery.ProjectName}.pdf"
            };
        }

        public async Task<IActionResult> ExportExcel(int id)
        {
            var taskQuery = await _db.Tasks
                .Where(t => t.ProjectId.ProjectId == id)
                .Include(user => user.AssignedTo)
                .Select(task => new
                {
                    task.TaskTitle,
                    task.TaskDescription,
                    task.AssignedTo,
                    CreatedOn = task.CreatedOn.ToString("dd/MM/yyyy"),
                    DueDate =  task.DueDate.ToString("dd/MM/yyyy"),
                    task.IsCompleted
                })
                .ToListAsync();
            var projectQuery = await _db.Projects.FindAsync(id);

            using (ExcelPackage package = new ExcelPackage()) 
            {
                //Row & Col
                var worksheet = package.Workbook.Worksheets.Add("Task");
                
                worksheet.Cells[4, 2].Value = projectQuery.ProjectName;
                worksheet.Cells[5, 2].Value = projectQuery.ProjectDescription;
                worksheet.Cells[7, 2].Value = projectQuery.DueDate.ToString("dd/MM/yyyy");
                worksheet.Cells[8, 2].Value = projectQuery.CreatedDate.ToString("dd/MM/yyyy");

                worksheet.Cells[10, 2].LoadFromCollection(taskQuery, true);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var filename = $"Report-{projectQuery.ProjectName}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var content = package.GetAsByteArray();

                return File(content, contentType, filename);
            }
        }
    }
}
