using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentTeacherManagement.Models.Identity;

namespace StudentTeacherManagement.Models.Entities
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey("StudentId")]
        public ApplicationUser Student { get; set; } = default!;

        [Required]
        public int AssignmentId { get; set; }

        [ForeignKey("AssignmentId")]
        public Assignment Assignment { get; set; } = default!;

        [Required]
        public string Content { get; set; } = string.Empty;

        public float? Grade { get; set; }
        
        [NotMapped]
        public string? StudentName => Student?.FullName; 

    }
}