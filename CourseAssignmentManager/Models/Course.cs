using System.ComponentModel.DataAnnotations;

namespace CourseAssignmentManager.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public int TeacherId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}