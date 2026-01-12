using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserApiController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.Initials,
                    Roles = roles,
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now
                });
            }

            return Ok(userDtos);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            if (string.IsNullOrEmpty(request.Role))
                return BadRequest(new { message = "Role is required" });

            if (!await _roleManager.RoleExistsAsync(request.Role))
                return BadRequest(new { message = "Role does not exist" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);

            return Ok(new { message = "Role updated successfully" });
        }

        [HttpPut("{id}/lock")]
        public async Task<IActionResult> ToggleUserLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now)
            {
                // Unlock
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            else
            {
                // Lock
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(100));
            }

            return Ok(new { 
                message = "User lock status updated", 
                isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now 
            });
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; }
    }
}
