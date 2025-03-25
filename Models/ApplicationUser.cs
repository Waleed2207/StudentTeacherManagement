using Microsoft.AspNetCore.Identity;

namespace StudentTeacherManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        // public string Role { get; set; }
        public string? FullName { get; set; }
    }
}