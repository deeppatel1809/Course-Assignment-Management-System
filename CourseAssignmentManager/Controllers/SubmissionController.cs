using Microsoft.AspNetCore.Mvc;
using CourseAssignmentManager.Data;
using CourseAssignmentManager.Models;

namespace CourseAssignmentManager.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SubmissionController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Submission/SubmitAssignment/{id}
        public IActionResult SubmitAssignment(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == id);

            if (assignment == null)
            {
                return RedirectToAction("AssignmentList", "Assignment");
            }

            ViewBag.Course = _context.Courses.FirstOrDefault(c => c.CourseId == assignment.CourseId);
            ViewData["Title"] = "Submit Assignment";

            return View(assignment);
        }

        // POST: /Submission/SubmitAssignment
        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(int assignmentId, IFormFile file)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            int studentId = HttpContext.Session.GetInt32("UserId") ?? 0;

            bool alreadySubmitted = _context.Submissions
                .Any(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

            if (alreadySubmitted)
            {
                return RedirectToAction("AssignmentList", "Assignment");
            }

            if (file == null || file.Length == 0)
            {
                return RedirectToAction("SubmitAssignment", new { id = assignmentId });
            }

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{studentId}_{assignmentId}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                FilePath = uniqueFileName
            };

            _context.Submissions.Add(submission);
            _context.SaveChanges();

            TempData["Success"] = "Assignment submitted successfully!";
            return RedirectToAction("AssignmentList", "Assignment");
        }

        // GET: /Submission/ViewSubmissions/{id}
        public IActionResult ViewSubmissions(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
            {
                return RedirectToAction("Main", "Dashboard");
            }

            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == id);

            if (assignment == null)
            {
                return RedirectToAction("AssignmentList", "Assignment");
            }

            var submissions = _context.Submissions
                .Where(s => s.AssignmentId == id)
                .OrderByDescending(s => s.SubmittedAt)
                .ToList();

            var studentIds = submissions
                .Select(s => s.StudentId)
                .Distinct()
                .ToList();

            var students = _context.Users
                .Where(u => studentIds.Contains(u.UserId))
                .ToList();

            ViewBag.Assignment = assignment;
            ViewBag.Students = students;
            ViewData["Title"] = "View Submissions";

            return View(submissions);
        }
    }
}