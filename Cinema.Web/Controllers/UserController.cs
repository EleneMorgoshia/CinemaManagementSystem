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


        /// <summary>
        /// returns all users. Only admins and super admins can call this endpoint.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>

        [HttpGet] 
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        /// <summary>
        /// returns a user by Id. Users can view their own profile. Admins and SuperAdmins can view any profile.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>The requested user's details</returns>
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


        /// <summary>
        /// Updates a user's profile. 
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="updateUserDto">The new profile details.ი</param>
        /// <returns>The updated user's details</returns>
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

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns></returns>
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

        /// <summary>
        ///  Assigns the Admin role to a user. Only super admins can do this.
        /// </summary>
        /// <param name="userId">The ID of the user receiving the Admin role</param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes the Admin role from a user. Only super admins can do this.
        /// </summary>
        /// <param name="userId">The ID of the user whose Admin role will be removed</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a user's roles. Admins and SuperAdmins can view them
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>The user's roles.</returns>
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoleById(string id)
        {
            var userRoles = await _userService.GetUserRoleByIdAsync(id);
            return Ok(userRoles);

        }
    }
}
