using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase //ამით ვეუბნები რომ დამიბრუნოს მარტო  HTTP სტატუსები
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet] //read
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id:int}")] //read
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        [HttpPost] //create
        public async Task<ActionResult<UserDTO>> AddUser(CreateUserDTO userDTO, CancellationToken cancellationToken)
        {
            if(userDTO == null) 
                return BadRequest("Invalid data");
            var user = await _userService.AddUserAsync(userDTO, cancellationToken);
            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                user
            );
        }

        [HttpPut("{id:int}")] //update
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateUserDto, CancellationToken cancellationToken)
        {
            await _userService.UpdateUser(id, updateUserDto, cancellationToken);
            return NoContent(); //204
        }

        [HttpDelete("{id:int}")] //delete
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            await _userService.DeleteUser(id, cancellationToken);
            return NoContent(); //204
        }
        //public IActionResult Index()
        //{
            
        //}
    }
}
