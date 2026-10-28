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
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO userDTO, CancellationToken cancellationToken)
        {
            await _userService.RegistrUserAsync(userDTO, cancellationToken);
            return Ok("User Registered successfully");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginDTO loginDTO, CancellationToken cancellationToken)
        {
            var isAuthenticated = await _userService.LogInUserAsync(loginDTO,  cancellationToken);
            if (!isAuthenticated)
                return Unauthorized("Invalid email or password");
            return Ok("User logged in successfully");
        }
    }
}
