using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cinema.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]/[action]")]
    public class UserController : ControllerBase //ამით ვეუბნები რომ დამიბრუნოს მარტო  HTTP სტატუსები
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet] 
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id}")] 
        public async Task<IActionResult> GetUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ჩერეზ ტოკენიდან
            if (string.IsNullOrWhiteSpace(currentUserId))
                return Unauthorized();

            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && currentUserId != id)
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

     

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDto)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(currentUserId))
                    return Unauthorized();

                var updatedUser = await _userService.UpdateUserAsync(currentUserId, id, updateUserDto);
                return Ok(updatedUser);
            }
            catch (UnauthorizedAccessException) 
            {
                return Forbid();
            }
        }


        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(currentUserId))
                    return Unauthorized();

                await _userService.DeleteUserAsync(currentUserId, id);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("{userId}")]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            try
            {
                await _userService.MakeAdminAsync(userId);
                return Ok(new { message = "Admin role assigned successfully." });
            }
            catch(UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveAdminRole(string userId)
        {
            try
            {
                await _userService.RemoveAdminRoleAsync(userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoleById(string id)
        {
            var userRoles = await _userService.GetUserRoleByIdAsync(id);
            return Ok(userRoles);

        }
    }
}
