using Microsoft.AspNetCore.Mvc;
using CourseAssignmentManager.Data;
using CourseAssignmentManager.Models;

namespace CourseAssignmentManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserRole") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string name, string email, string password, string role)
        {
            bool emailExists = _context.Users.Any(u => u.Email == email);
            if (emailExists)
            {
                ViewBag.Error = "This email is already registered.";
                return View();
            }

            var user = new User
            {
                Name = name,
                Email = email,
                Password = password,
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Account created! Please login.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserRole") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password. Please try again.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Main", "Dashboard");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}