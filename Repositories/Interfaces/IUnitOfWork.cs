using System;
using System.Threading.Tasks;
using StudentTeacherManagement.Models;

namespace StudentTeacherManagement.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAssignmentRepository Assignments { get; }
        ISubmissionRepository Submissions { get; }
        IUserRepository Users { get; }   
        Task<int> SaveAsync();
    }
}
