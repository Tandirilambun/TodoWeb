using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Data;
using TodoWeb.Models;
using TodoWeb.TodoVM;

namespace TodoWeb.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        public TodoController(UserManager<User>userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int catId)
        {
            var userLog = await _userManager.GetUserAsync(User);

            var todoQuery = await _context.Todos
                .Where(todo => todo.User.Id == userLog.Id)
                .Include(category => category.Category)
                .Include(user => user.User)
                .OrderByDescending(update => update.UpdatedAt)
                .ToListAsync();

            var todoCategory = await _context.Category.ToListAsync();

            if (todoQuery == null)
            {
                return NotFound();
            }

            if (catId != 0)
            {
                todoQuery = todoQuery.Where(cat => cat.Category.Category_id == catId).ToList();
            }
            var todoVm = new TodoViewModel
            {
                Todoes = todoQuery,
                Categories = new SelectList(todoCategory, "Category_id", "Category_name"),
                Category_note_id = catId
            };
            return View(todoVm);
        }

        public async Task<IActionResult> Create()
        {
            var todoCategory = await _context.Category.ToListAsync();
            var todoVM = new TodoViewModel
            {
                TodoSingle = new Todo(),
                Categories = new SelectList(todoCategory, "Category_id", "Category_name")
            };
            return View(todoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTodo(TodoViewModel todoObj)
        {
            var selectedCategoryName = await Utils.Utils.GetCategoryAsync(_context, todoObj.TodoSingle.Category.Category_id);

            var user = await _userManager.GetUserAsync(User);

            var inputTodo = new Todo
            {
                User = user,
                Title = todoObj.TodoSingle.Title,
                Description = todoObj.TodoSingle.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsCompleted = todoObj.TodoSingle.IsCompleted,
                Category = selectedCategoryName,
            };
            _context.Todos.Add(inputTodo);
            await _context.SaveChangesAsync();
            TempData["success"] = "Todo successfully created";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound();
            }
            var todoCategory = await _context.Category.ToListAsync();
            var todoVm = new TodoViewModel
            {
                Categories = new SelectList(todoCategory,"Category_id", "Category_name"),
                Note_id = todo.Note_id,
                Note_title = todo.Title,
                Note_description = todo.Description,
                Note_completed = todo.IsCompleted,
                Category_note_id = todo.Category.Category_id
            };
            return View(todoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTodo(TodoViewModel todoUpdt)
        {
            var selectedCategory = await Utils.Utils.GetCategoryAsync(_context, todoUpdt.Category_note_id);
            var todo = await _context.Todos.FindAsync(todoUpdt.Note_id);
            if (todo == null) 
            { 
                return NotFound();
            }
            todo.Note_id = todoUpdt.Note_id;
            todo.Title = todoUpdt.Note_title;
            todo.Description = todoUpdt.Note_description;
            todo.UpdatedAt = DateTime.UtcNow;
            todo.IsCompleted = todoUpdt.Note_completed;
            todo.Category = selectedCategory;

            await _context.SaveChangesAsync();
            TempData["success"] = "Todo successfully Updated";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetDeleteData(int? id)
        {
            if(id == null)
            {
                return NotFound(); 
            }
            var todoItem = await _context.Todos.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }
            return Json(todoItem);
        }

        public async Task<IActionResult> DeleteTodo(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var todoItem = await _context.Todos.FindAsync(id);
            if (todoItem == null) { 
                return NotFound();
            }
            _context.Todos.Remove(todoItem);
            await _context.SaveChangesAsync();
            TempData["success"] = "Todo successfully deleted";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FilterTodo(int? categoriesValue)
        {
            var userLog = await _userManager.GetUserAsync(User);
            var todoCategory = await _context.Category.ToListAsync();

            var todoQuery = await _context.Todos
                .Where(todo => todo.User.Id == userLog.Id)
                .Where(cat => cat.Category.Category_id == categoriesValue)
                .Include(category => category.Category)
                .Include(user => user.User)
                .OrderByDescending(update => update.UpdatedAt)
                .ToListAsync();

            var todoVm = new TodoViewModel
            {
                Todoes = todoQuery,
                Categories = new SelectList(todoCategory, "Category_id", "Category_name")
            };

            return RedirectToAction("Index", todoVm);
        }
    }
}

