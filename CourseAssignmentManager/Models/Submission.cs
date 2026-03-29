using System.ComponentModel.DataAnnotations;

namespace CourseAssignmentManager.Models
{
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        public int AssignmentId { get; set; }

        public int StudentId { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}