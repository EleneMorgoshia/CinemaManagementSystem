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
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;
        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllSeats(CancellationToken cancellationToken)
        {
            var seats = await _seatService.GetAllSeatsAsync(cancellationToken);
            return Ok(seats);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeatById(int id, CancellationToken cancellationToken)
        {
            var seat = await _seatService.GetSeatByIdAsync(id, cancellationToken);
            return Ok(seat);
        }

        [HttpGet("{hallId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeatsByHallId(int hallId, CancellationToken cancellationToken)
        {
            var seats = await _seatService.GetSeatsByHallIdAsync(hallId, cancellationToken);
            return Ok(seats);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async  Task<IActionResult> AddSeat(CreateSeatDTO createSeatDTO, CancellationToken cancellationToken)
        {
            var seat = await _seatService.AddSeatAsync(createSeatDTO, cancellationToken);
            return CreatedAtAction
                (
                    nameof(GetSeatById),
                    new { id = seat.Id },
                    seat
                );
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateSeat(int id, UpdateSeatDTO updateSeatDTO, CancellationToken cancellationToken)
        {
            await _seatService.UpdateSeatAsync(id, updateSeatDTO, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteSeat(int id, CancellationToken cancellationToken)
        {
            await _seatService.DeleteSeatAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
