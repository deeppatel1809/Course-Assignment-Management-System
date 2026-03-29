using Microsoft.AspNetCore.Mvc;
using CourseAssignmentManager.Data;
using CourseAssignmentManager.Models;

namespace CourseAssignmentManager.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly AppDbContext _context;

        public AssignmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Assignment/AssignmentList
        public IActionResult AssignmentList()
        {
            string? role = HttpContext.Session.GetString("UserRole");

            if (role == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            List<Assignment> assignments;

            if (role == "Teacher")
            {
                var teacherCourseIds = _context.Courses
                    .Where(c => c.TeacherId == userId)
                    .Select(c => c.CourseId)
                    .ToList();

                assignments = _context.Assignments
                    .Where(a => teacherCourseIds.Contains(a.CourseId))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();
            }
            else
            {
                assignments = _context.Assignments
                    .OrderByDescending(a => a.DueDate)
                    .ToList();
            }

            ViewBag.Courses = _context.Courses.ToList();

            if (role == "Student")
            {
                ViewBag.MySubmissions = _context.Submissions
                    .Where(s => s.StudentId == userId)
                    .Select(s => s.AssignmentId)
                    .ToList();
            }

            ViewData["Title"] = "Assignments";

            return View(assignments);
        }

        // GET: /Assignment/CreateAssignment
        public IActionResult CreateAssignment()
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            ViewBag.Courses = _context.Courses
                .Where(c => c.TeacherId == userId)
                .ToList();

            ViewData["Title"] = "Create Assignment";

            return View();
        }

        // POST: /Assignment/CreateAssignment
        [HttpPost]
        public IActionResult CreateAssignment(string assignmentName, string description, int courseId, DateTime dueDate)
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            var assignment = new Assignment
            {
                AssignmentName = assignmentName,
                Description = description ?? string.Empty,
                CourseId = courseId,
                DueDate = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc)
            };

            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            TempData["Success"] = "Assignment created successfully!";
            return RedirectToAction("AssignmentList");
        }

        // GET: /Assignment/DeleteAssignment/{id}
        public IActionResult DeleteAssignment(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == id);

            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
                _context.SaveChanges();
                TempData["Success"] = "Assignment deleted.";
            }

            return RedirectToAction("AssignmentList");
        }
    }
}