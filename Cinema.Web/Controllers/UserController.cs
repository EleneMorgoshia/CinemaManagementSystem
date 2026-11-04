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

        [HttpGet] //read
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id}")] //read
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ჩერეზ ტოკენიდან
            if (string.IsNullOrWhiteSpace(currentUserId))
                return Unauthorized();

            if (!User.IsInRole("Admin") && currentUserId != id)
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

     

        [HttpPut("{id}")] //update
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != id)
                return Forbid();

            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);
            return Ok(updatedUser);
        }


        [HttpDelete("{id}")] //delete
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != id)
                return Forbid();

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
