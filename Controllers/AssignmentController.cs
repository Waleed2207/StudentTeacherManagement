using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTeacherManagement.Data;
using StudentTeacherManagement.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/assignments")]
[ApiController]
public class AssignmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssignmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAssignments()
    {
        var assignments = await _context.Assignments.Include(a => a.Submissions).ToListAsync();
        return Ok(assignments);
    }

    [HttpPost]
    [Authorize(Policy = "TeacherPolicy")]
    public async Task<IActionResult> CreateAssignment([FromBody] Assignment assignment)
    {
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();
        return Ok(assignment);
    }

    [HttpPost("{id}/submit")]
    [Authorize(Policy = "StudentPolicy")]
    public async Task<IActionResult> SubmitAssignment(int id, [FromBody] Submission submission)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment == null) return NotFound();

        submission.AssignmentId = id;
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return Ok(submission);
    }
}