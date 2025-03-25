using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentTeacherManagement.Models.Identity;

namespace StudentTeacherManagement.Models.Entities
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        
        public string? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public ApplicationUser? Teacher { get; set; } 
        
        public DateTime? Deadline { get; set; }
        
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        
        [NotMapped]
        public string? TeacherName => Teacher?.FullName; 
        public string CourseName { get; set; } = string.Empty;

    }

}