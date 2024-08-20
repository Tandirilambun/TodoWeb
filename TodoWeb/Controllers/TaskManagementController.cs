using Microsoft.AspNetCore.Mvc;

namespace TodoWeb.Controllers
{
    public class TaskManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
