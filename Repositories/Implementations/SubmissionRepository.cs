using StudentTeacherManagement.Data;
using StudentTeacherManagement.Models.Entities;
using StudentTeacherManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentTeacherManagement.Repositories.Implementations
{
    public class SubmissionRepository : Repository<Submission>, ISubmissionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubmissionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ✅ Implement missing method
        public async Task<IEnumerable<Submission>> GetAllAsync()
        {
            return await _context.Submissions.ToListAsync();
        }

        public async Task<Submission?> GetByIdAsync(int id)
        {
            return await _context.Submissions.FindAsync(id);

        }

        public Task FindAsync(Func<object, bool> func)
        {
            throw new NotImplementedException();
        }

        public object GetAll()
        {
            throw new NotImplementedException();
        }
    }
}