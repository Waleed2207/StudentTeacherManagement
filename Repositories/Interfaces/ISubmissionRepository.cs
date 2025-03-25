using StudentTeacherManagement.Models;
using System.Threading.Tasks;
using StudentTeacherManagement.Models.Entities;

namespace StudentTeacherManagement.Repositories.Interfaces
{
    public interface ISubmissionRepository : IRepository<Submission>
    {
        Task<IEnumerable<Submission>> GetAllAsync();
        Task<Submission?> GetByIdAsync(int id); // <-- Add this
        Task AddAsync(Submission submission);
        Task FindAsync(Func<object, bool> func);
    }

}