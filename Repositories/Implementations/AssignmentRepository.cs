using System.Collections;
using System.Linq.Expressions;
using StudentTeacherManagement.Data;
using StudentTeacherManagement.Models.Entities;
using StudentTeacherManagement.Repositories.Interfaces;

namespace StudentTeacherManagement.Repositories.Implementations
{
    public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _context.Assignments.FindAsync(id);
        }

        public Task<IEnumerable> FindAsync(Func<object, bool> func)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Assignment>> FindAsync(Expression<Func<Assignment, bool>> predicate)
        {
            throw new NotImplementedException();
        }
        public new IQueryable<Assignment> GetAll()
        {
            return _context.Assignments.AsQueryable();
        }
    }

}
