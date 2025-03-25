using StudentTeacherManagement.Models.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using StudentTeacherManagement.Models;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<IEnumerable<ApplicationUser>> GetAllTeachersAsync();
    
    
}