using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cinema.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDTO">The user's registration details.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>>A message confirming that the account was created</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO userDTO, CancellationToken cancellationToken)
        {
            await _userService.RegistrUserAsync(userDTO, cancellationToken);
            return Ok("User Registered successfully");
        }

        /// <summary>
        /// Signs in the user with their email and password and returns a JWT.
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>>A JWT on success, or HTTP 401 if the email or password is incorrect</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginUserAsync(loginDTO, cancellationToken);
            if (result == null)
                return Unauthorized("Invalid email or password");
            return Ok(result);
        }

        /// <summary>
        /// Handles the user's logout request.
        /// </summary>
        /// <returns>HTTP 204 No Content</returns>
        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            return NoContent();
        }

        /// <summary>
        /// Changes the signed-in user's password.
        /// </summary>
        /// <param name="changePasswordDTO">The current password and the new password.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ჩერეზ ტოკენიდან ამოვიღე იუზერის აიდი
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _authService.ChangePasswordAsync(userId, changePasswordDTO);
            return NoContent();
        }
    }
}
