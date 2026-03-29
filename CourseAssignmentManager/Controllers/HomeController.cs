using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CourseAssignmentManager.Models;

namespace CourseAssignmentManager.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != null)
            {
                return RedirectToAction("Main", "Dashboard");
            }
            return View();
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}