using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagement.Models;

namespace StudentTeacherManagement.Controllers;

[Route("api/admin")]
[ApiController]
[Authorize(Policy = "AdminPolicy")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/admin/users
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users.Select(u => new { u.Id, u.UserName, u.Email });
        return Ok(users);
    }

    // PATCH: api/admin/users/{userId}/role
    [HttpPatch("users/{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(string userId, [FromBody] string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user, newRole);

        return Ok(new { message = $"User promoted to {newRole}" });
    }
}
