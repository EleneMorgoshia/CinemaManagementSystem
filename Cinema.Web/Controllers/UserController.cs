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
        private readonly IUsermManagerService _userService;
        public UserController(IUsermManagerService userService)
        {
            _userService = userService;
        }

        [HttpGet] //read
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id}")] //read
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

     

        [HttpPut("{id}")] //update
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != id)
                return Forbid();

            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);
            return Ok(updatedUser);
        }


        [HttpDelete("{id}")] //delete
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != id)
                return Forbid();

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
