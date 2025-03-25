using StudentTeacherManagement.Data;
using StudentTeacherManagement.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

using StudentTeacherManagement.Models.Identity;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IAssignmentRepository Assignments { get; }
    public ISubmissionRepository Submissions { get; }
    public IUserRepository Users { get; } // ✅ Add this

    public UnitOfWork(ApplicationDbContext context, 
        IAssignmentRepository assignments, 
        ISubmissionRepository submissions,
        IUserRepository users) // ✅ Inject users repo
    {
        _context = context;
        Assignments = assignments;
        Submissions = submissions;
        Users = users; // ✅ Assign it
    }

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
