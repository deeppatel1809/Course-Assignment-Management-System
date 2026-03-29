using Microsoft.AspNetCore.Mvc;
using CourseAssignmentManager.Data;
using CourseAssignmentManager.Models;

namespace CourseAssignmentManager.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        //  GET: /Course/CourseList
        public IActionResult CourseList()
        {
            string? role = HttpContext.Session.GetString("UserRole");

            if (role == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            List<Course> courses;

            if (role == "Teacher")
            {
                courses = _context.Courses
                    .Where(c => c.TeacherId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
            }
            else
            {
                courses = _context.Courses
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
            }

            ViewBag.Teachers = _context.Users
                .Where(u => u.Role == "Teacher")
                .ToList();

            ViewData["Title"] = "Courses";

            return View(courses);
        }

        //  GET: /Course/CreateCourse
        public IActionResult CreateCourse()
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            ViewData["Title"] = "Create Course";

            return View();
        }

        //  POST: /Course/CreateCourse
        [HttpPost]
        public IActionResult CreateCourse(string courseName, string description)
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            int teacherId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var course = new Course
            {
                CourseName = courseName,
                Description = description ?? string.Empty,
                TeacherId = teacherId
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction("CourseList");
        }

        //  GET: /Course/DeleteCourse/{id}
        public IActionResult DeleteCourse(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);

            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
                TempData["Success"] = "Course deleted.";
            }

            return RedirectToAction("CourseList");
        }
    }
}