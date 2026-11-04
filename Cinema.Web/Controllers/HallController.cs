using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HallController : ControllerBase
    {
        private readonly IHallService _hallService;
        public HallController(IHallService hallService)
        {
            _hallService = hallService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllHalls(CancellationToken cancellationToken)
        {
            var halls = await _hallService.GetAllHallsAsync(cancellationToken);
            return Ok(halls);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHallById(int id, CancellationToken cancellationToken)
        {
            var hall = await _hallService.GetHallByIdAsync(id, cancellationToken);
            return Ok(hall);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddHall(CreateHallDTO createHallDTO, CancellationToken cancellationToken)
        {
            var hall = await _hallService.AddHallAsync(createHallDTO, cancellationToken);
            return CreatedAtAction
                (
                    nameof(GetHallById),
                    new { id = hall.Id },
                    hall
                );
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHall(int id, UpdateHallDTO updateHallDTO, CancellationToken cancellationToken)
        {
            await _hallService.UpdateHallAsync(id, updateHallDTO, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHall(int id, CancellationToken cancellationToken)
        {
            await _hallService.DeleteHallAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
