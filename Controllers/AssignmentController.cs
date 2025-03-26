// AssignmentsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagement.Models.Entities;
using StudentTeacherManagement.Repositories.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudentTeacherManagement.Controllers
{
    [Route("api/assignments")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // ✅ GET: api/assignments - View Assignments (Teachers & Students)
        [HttpGet]
        [Authorize(Policy = "TeacherPolicy")]
        public async Task<IActionResult> GetAssignmentsByTeacherId()
        {
            try
            {
                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(teacherId))
                    return Unauthorized(new { message = "Invalid user session" });

                var assignments = await _unitOfWork.Assignments
                    .GetAll()
                    .Where(a => a.TeacherId == teacherId)
                    .Include(a => a.Teacher)
                    .Include(a => a.Submissions)
                    .ThenInclude(s => s.Student)
                    .ToListAsync();

                var result = assignments.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.Deadline,
                    TeacherName = a.Teacher?.FullName,
                    
                    Submissions = a.Submissions.Select(s => new
                    {
                        s.Id,
                        s.StudentId,
                        s.Content,
                        s.Grade,
                        StudentName = s.Student?.FullName
                    })
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching assignments", error = ex.Message });
            }
        }
        
        // GET: api/assignments/teachers
        [HttpGet("teachers")]
        [Authorize(Policy = "StudentPolicy")]
        public async Task<IActionResult> GetTeachersWithAssignments()
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(studentId))
                    return Unauthorized(new { message = "Invalid user session" });

                var assignments = await _unitOfWork.Assignments
                    .GetAll()
                    .Include(a => a.Teacher)
                    .Include(a => a.Submissions)
                    .ToListAsync();

                var grouped = assignments
                    .Where(a => a.Teacher != null)
                    .GroupBy(a => new { a.TeacherId, a.Teacher.FullName, a.CourseName })
                    .Select(group => new
                    {
                        TeacherId = group.Key.TeacherId,
                        TeacherName = group.Key.FullName,
                        CourseName = group.Key.CourseName,
                        Assignments = group.Select(a =>
                        {
                            var submission = a.Submissions.FirstOrDefault(s => s.StudentId == studentId);
                            return new
                            {
                                a.Id,
                                a.Title,
                                a.Description,
                                a.Deadline,
                                a.CourseName,
                                Grade = submission?.Grade,
                                Content = submission?.Content
                            };
                        })
                    });

                return Ok(grouped);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching teachers and assignments", error = ex.Message });
            }
        }

       // ✅ GET: api/assignments - View Assignments (Teachers & Students)
      //  Fix in AssignmentsController - GetAssignments()
        [HttpGet("admin")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAssignments()
        {
            try
            {
                var assignments = await _unitOfWork.Assignments.GetAll()
                    .Include(a => a.Teacher)
                    .Include(a => a.Submissions)
                    .ThenInclude(s => s.Student)
                    .ToListAsync();
        
                var result = assignments.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.Deadline,
                    TeacherName = a.Teacher?.FullName,
                    Submissions = a.Submissions.Select(s => new
                    {
                        s.Id,
                        s.StudentId,
                        s.Content,
                        s.Grade,
                        StudentName = s.Student?.FullName
                    })
                });
        
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching assignments", error = ex.Message });
            }
        }

 
        // ✅ POST: api/assignments - Create Assignment (Teacher only) 
        [HttpPost]
        [Authorize(Policy = "TeacherPolicy")]
        public async Task<IActionResult> CreateAssignment([FromBody] Assignment assignment)
        {
            try
            {
                if (assignment == null)
                    return BadRequest(new { message = "Invalid assignment data" });

                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(teacherId))
                    return Unauthorized(new { message = "Invalid user session" });

                assignment.TeacherId = teacherId;

                await _unitOfWork.Assignments.AddAsync(assignment);
                await _unitOfWork.SaveAsync();

                return CreatedAtAction(nameof(GetAssignmentsByTeacherId), new { id = assignment.Id }, assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating assignment", error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        
        // ✅ PUT: api/assignments/{id} - Edit Assignment (Teacher only)
        [HttpPut("{id}")]
        [Authorize(Policy = "TeacherPolicy")]
        public async Task<IActionResult> EditAssignment(int id, [FromBody] Assignment updatedAssignment)
        {
            try
            {
                var existing = await _unitOfWork.Assignments.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Assignment not found" });

                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (teacherId != existing.TeacherId)
                    return Forbid("You can only edit your own assignments.");

                existing.Title = updatedAssignment.Title;
                existing.Description = updatedAssignment.Description;
                existing.Deadline = updatedAssignment.Deadline;
                await _unitOfWork.SaveAsync();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating assignment", error = ex.Message });
            }
        }
        
        // ✅ DELETE: api/assignments/{id} - Delete Assignment (Teacher only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            try
            {
                var assignment = await _unitOfWork.Assignments.GetByIdAsync(id);
                if (assignment == null)
                    return NotFound(new { message = "Assignment not found" });

                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (teacherId != assignment.TeacherId)
                    return StatusCode(403, new { message = "You can only delete your own assignments." });

                _unitOfWork.Assignments.Delete(assignment);
                await _unitOfWork.SaveAsync();

                return Ok(new { message = "Assignment deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting assignment", error = ex.Message });
            }
        }
    }
}
