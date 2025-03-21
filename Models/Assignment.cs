using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentTeacherManagement.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TeacherId { get; set; }
        public List<Submission> Submissions { get; set; } = new List<Submission>();

    }
}