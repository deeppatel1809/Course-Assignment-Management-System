using Microsoft.AspNetCore.Mvc;
using CourseAssignmentManager.Data;

namespace CourseAssignmentManager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        //  GET: /Dashboard/Main
        public IActionResult Main()
        {
            string? role = HttpContext.Session.GetString("UserRole");

            if (role == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (role == "Teacher")
            {
                var myCourses = _context.Courses
                    .Where(c => c.TeacherId == userId)
                    .ToList();

                var myCourseIds = myCourses.Select(c => c.CourseId).ToList();

                var myAssignments = _context.Assignments
                    .Where(a => myCourseIds.Contains(a.CourseId))
                    .ToList();

                var myAssignmentIds = myAssignments.Select(a => a.AssignmentId).ToList();

                var submissions = _context.Submissions
                    .Where(s => myAssignmentIds.Contains(s.AssignmentId))
                    .ToList();

                var recentSubmissions = submissions
                    .OrderByDescending(s => s.SubmittedAt)
                    .Take(5)
                    .ToList();

                var recentStudentIds = recentSubmissions
                    .Select(s => s.StudentId)
                    .Distinct()
                    .ToList();

                var recentStudents = _context.Users
                    .Where(u => recentStudentIds.Contains(u.UserId))
                    .ToList();

                var recentAssignmentIds = recentSubmissions
                    .Select(s => s.AssignmentId)
                    .Distinct()
                    .ToList();

                var recentAssignments = myAssignments
                    .Where(a => recentAssignmentIds.Contains(a.AssignmentId))
                    .ToList();

                ViewData["Title"] = "Teacher Dashboard";
                ViewBag.CoursesCount = myCourses.Count;
                ViewBag.AssignmentsCount = myAssignments.Count;
                ViewBag.SubmissionsCount = submissions.Count;
                ViewBag.RecentSubmissions = recentSubmissions;
                ViewBag.RecentStudents = recentStudents;
                ViewBag.RecentAssignments = recentAssignments;

                return View("TeacherDashboard");
            }
            else
            {
                var allCourses = _context.Courses.ToList();
                var allAssignments = _context.Assignments.ToList();

                var mySubmissions = _context.Submissions
                    .Where(s => s.StudentId == userId)
                    .ToList();

                var recentAssignments = allAssignments
                    .OrderBy(a => a.DueDate)
                    .Take(5)
                    .ToList();

                var recentCourseIds = recentAssignments
                    .Select(a => a.CourseId)
                    .Distinct()
                    .ToList();

                var recentCourses = allCourses
                    .Where(c => recentCourseIds.Contains(c.CourseId))
                    .ToList();

                var submittedAssignmentIds = mySubmissions
                    .Select(s => s.AssignmentId)
                    .ToList();

                ViewData["Title"] = "Student Dashboard";
                ViewBag.CoursesCount = allCourses.Count;
                ViewBag.AssignmentsCount = allAssignments.Count;
                ViewBag.SubmittedCount = mySubmissions.Count;
                ViewBag.RecentAssignments = recentAssignments;
                ViewBag.RecentCourses = recentCourses;
                ViewBag.SubmittedAssignmentIds = submittedAssignmentIds;

                return View("StudentDashboard");
            }
        }
    }
}