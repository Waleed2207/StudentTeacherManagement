using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTeacherManagement.Models
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }
        public string StudentId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [ForeignKey("AssignmentId")]
        public Assignment Assignment { get; set; }

        [Required]
        public string Content { get; set; } 
        
        public int Grade { get; set; }

    }
}