using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Data;
using TodoWeb.Models;
using TodoWeb.TodoVM;

namespace TodoWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginController(SignInManager<User> signInManager, UserManager<User> userManager, ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;

        }
        public IActionResult Index(AuthVM model)
        {
            return View(model);
        }

        public async Task<IActionResult> Register(string regName, string regUsername, string regEmail, string phoneNumber, string regPassword)
        {

            var userByUsername = await _userManager.FindByNameAsync(regUsername);
            if (userByUsername != null)
            {
                ModelState.AddModelError("Username", "Username already taken");
                return View(userByUsername);
            }

            var userByEmail = await _userManager.FindByEmailAsync(regEmail);
            if (userByEmail != null)
            {
                ModelState.AddModelError("User_email", "Email already used");
                return View(userByEmail);
            }
            var user = new User
            {
                Name = regName,
                UserName = regUsername,
                Email = regEmail,
                PhoneNumber = phoneNumber,
            };
            var result = await _userManager.CreateAsync(user, regPassword);
            if (result.Succeeded)
            {
                TempData["success"] = "User successfully created";
                return RedirectToAction("Index", "Login");
            }
            return RedirectToAction("Index", "Login");
        }

        public async Task<IActionResult> LoginUser(AuthVM login)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Username or Password Invalid");
                return View("Index", login);
            }

            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(login.UserName);
            }
            var loginResult = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, login.KeepLogin, false);

            if (loginResult.Succeeded)
            {
                TempData["login"] = "Login Successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("fail", "Username or Password Incorrect");
            }
            return View("Index", login);
        }

        public async Task<IActionResult> LogoutUser()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}
