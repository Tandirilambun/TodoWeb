using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Data;
using TodoWeb.Models;
using TodoWeb.TodoVM;

namespace TodoWeb.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<User> _userManager;

        public ProjectController(UserManager<User> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userLogin = _userManager.GetUserId(User);

            var projectQuery = await _db.Projects
                .Where(proj => proj.Participants.Any(user => user.UserId == userLogin))
                .Include(proj => proj.Participants)
                .ThenInclude(user => user.User)
                .ToListAsync();

            var viewModel = new ProjectVM
            {
                Projects = projectQuery
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create(ProjectVM model)
        {
            var userQuery = await _db.Users.ToListAsync();
            var viewModel = new ProjectVM
            {
                Users = userQuery,
            };
            return View(viewModel);
        }

        public async Task<IActionResult> CreateProject(ProjectVM model, List<string> participant)
        {
            if (model.ProjectsAdd != null && participant.Count != 0) {
                var userCreate = await _userManager.GetUserAsync(User);
                var dueDate = DateOnly.Parse(model.DueDate);
                var project = new Projects
                {
                    ProjectName = model.ProjectsAdd.ProjectName,
                    ProjectDescription = model.ProjectsAdd.ProjectDescription,
                    CreatedDate = Utils.Utils.GetJktTime(),
                    CreatedBy = userCreate,
                    DueDate = dueDate,
                };
                await _db.Projects.AddAsync(project);
                await _db.SaveChangesAsync();

                foreach (var userParticipate in participant)
                {
                    var part = new Participants
                    {
                        ProjectId = project.ProjectId,
                        UserId = userParticipate,
                    };

                    _db.Participants.Add(part);
                }
                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Project");
            }
            return View(model);
        }
    }
}
