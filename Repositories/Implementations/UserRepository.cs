using StudentTeacherManagement.Data;
using StudentTeacherManagement.Models.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentTeacherManagement.Models;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllTeachersAsync()
    {
        return await _context.Users
            .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == "1")) // RoleId "1" = Teacher
            .ToListAsync();
    }
}