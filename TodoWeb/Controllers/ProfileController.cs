using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoWeb.Models;
using TodoWeb.Services;
using TodoWeb.TodoVM;
namespace TodoWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly FirebaseStorageService _firebaseStorageService;
        private readonly UserManager<User> _userManager;

        public ProfileController(FirebaseStorageService firebaseStorageService, UserManager<User> userManager)
        {
            _firebaseStorageService = firebaseStorageService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(ProfileVM vm, string id)
        {
            var userQuery = await _userManager.FindByIdAsync(id);
            if (userQuery == null) { 
                return NotFound();
            }

            vm = new ProfileVM
            {
                Profile = userQuery
            };

            return View(vm);
        }

        public async Task<IActionResult> UploadProfilePicture(ProfileVM obj)
        {
            var user = await _userManager.GetUserAsync(User);
            var objectName = $"profile-pictures/{user.Id}";

            var fileUrl = await _firebaseStorageService.UploadFileAsync(obj.formFile, user.Id, objectName, obj.formFile.ContentType);

            user.ProfilePhotoPath = fileUrl;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) {
                TempData["fail"] = "Upload Profile Photos Failed";
                return RedirectToAction("Index", "Profile");
            }

            TempData["success"] = "Upload Profile Photos Success";
            return Redirect($"/Profile/Index/{user.Id}");
        }
    }
}
