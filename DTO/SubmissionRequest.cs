using System.ComponentModel.DataAnnotations;

namespace StudentTeacherManagement.DTO;

public class SubmissionRequestDto
{
    [Required]
    public string Content { get; set; } = string.Empty;
}
