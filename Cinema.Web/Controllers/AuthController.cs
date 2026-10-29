using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO userDTO, CancellationToken cancellationToken)
        {
            await _userService.RegistrUserAsync(userDTO, cancellationToken);
            return Ok("User Registered successfully");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginUserAsync(loginDTO, cancellationToken);
            if (result == null)
                return Unauthorized("Invalid email or password");
            return Ok(result);
        }
    }
}
