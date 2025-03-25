using System.Collections;
using StudentTeacherManagement.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StudentTeacherManagement.Models.Entities;

namespace StudentTeacherManagement.Repositories.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment> GetByIdAsync(int id);
        Task AddAsync(Assignment assignment);
        void Delete(Assignment assignment);
        Task<IEnumerable> FindAsync(Func<object, bool> func);
        IQueryable<Assignment> GetAll(); // ⬅️ Add this if it's not there

        Task<IEnumerable<Assignment>> FindAsync(Expression<Func<Assignment, bool>> predicate);
    }
}