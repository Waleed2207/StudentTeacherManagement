// SubmissionsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagement.DTO;
using StudentTeacherManagement.Models.Entities;
using StudentTeacherManagement.Repositories.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudentTeacherManagement.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmissionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        // ✅ POST: api/submissions/{assignmentId}/submit - Submit Assignment (Student only)
        [HttpPost("{assignmentId}/submit")]
        [Authorize(Policy = "StudentPolicy")]
        public async Task<IActionResult> SubmitAssignment(int assignmentId, [FromBody] SubmissionRequestDto request)
        {
            var assignment = await _unitOfWork.Assignments.GetByIdAsync(assignmentId);
            if (assignment == null)
                return NotFound(new { message = "Assignment not found" });

            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new { message = "Invalid user session" });

            // ✅ Check if submission already exists for this student and assignment
            var existingSubmission = await _unitOfWork.Submissions
                .GetAll()
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
 
            if (existingSubmission != null)
            {
                return Conflict(new { message = "You have already submitted this assignment." });
            }

            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                Content = request.Content
            };

            await _unitOfWork.Submissions.AddAsync(submission);
            await _unitOfWork.SaveAsync();

            return Ok(new
            {
                message = "Submission successful",
                submission = new
                {
                    submission.Id,
                    submission.AssignmentId,
                    submission.Content,
                    submission.Grade
                }
            });
        }        
        [HttpGet("{assignmentId}")]
        [Authorize(Policy = "TeacherPolicy")]
        public async Task<IActionResult> GetSubmissions(int assignmentId)
        {
            var assignment = await _unitOfWork.Assignments.GetByIdAsync(assignmentId);
            if (assignment == null)
                return NotFound(new { message = "Assignment not found" });

            var submissions = (await _unitOfWork.Submissions.GetAllAsync())
                .Where(s => s.AssignmentId == assignmentId)
                .ToList();

            return Ok(submissions);
        }
        
        // ✅ GET: api/submissions/my-submissions - View student's own submissions
        [HttpGet("my-submissions")]
        [Authorize(Policy = "StudentPolicy")]
        public async Task<IActionResult> GetMySubmissions()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new { message = "Invalid user session" });

            var mySubmissions = (await _unitOfWork.Submissions.GetAllAsync())
                .Where(s => s.StudentId == studentId)
                .Select(s => new
                {
                    s.Id,
                    s.AssignmentId,
                    s.Content,
                    s.Grade,
                    StudentName = s.Student?.FullName // ✅ fixed here
                });

            return Ok(mySubmissions);
        }
        
        // ✅ PATCH: api/submissions/{assignmentId}/grade/{submissionId} - Grade submission (Teacher only)
        [HttpPatch("{assignmentId}/grade/{submissionId}")]
        [Authorize(Policy = "TeacherPolicy")]
        public async Task<IActionResult> GradeSubmission(int assignmentId, int submissionId, [FromBody] float grade)
        {
            try
            {
                var submission = await _unitOfWork.Submissions.GetByIdAsync(submissionId);
                if (submission == null || submission.AssignmentId != assignmentId)
                    return NotFound(new { message = "Submission not found for this assignment" });

                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var assignment = await _unitOfWork.Assignments.GetByIdAsync(assignmentId);
                if (assignment == null || assignment.TeacherId != teacherId)
                    return Forbid("You can only grade submissions for your own assignments.");

                submission.Grade = grade;
                await _unitOfWork.SaveAsync();

                return Ok(new { message = "Submission graded", submission });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error grading submission", error = ex.Message });
            }
        }
    }
}
