using System.ComponentModel.DataAnnotations;

namespace CourseAssignmentManager.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        [Display(Name = "Assignment Title")]
        public string AssignmentName { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public int CourseId { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}