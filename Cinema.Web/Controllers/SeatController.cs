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

        /// <summary>
        /// Returns all seats. Anyone can view them.
        /// </summary>
        /// <param></param>
        /// <returns>A list of all seats.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllSeats(CancellationToken cancellationToken)
        {
            var seats = await _seatService.GetAllSeatsAsync(cancellationToken);
            return Ok(seats);
        }

        /// <summary>
        /// Returns a seat by ID. Anyone can view it.
        /// </summary>
        /// <param name="id">The ID of the seat to retrieve</param>
        /// <param></param>
        /// <returns>The requested seat.</returns>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeatById(int id, CancellationToken cancellationToken)
        {
            var seat = await _seatService.GetSeatByIdAsync(id, cancellationToken);
            return Ok(seat);
        }

        /// <summary>
        /// Returns all seats in a cinema hall. Anyone can view them.
        /// </summary>
        /// <param name="hallId">The ID of the hall whose seats to retrieve.</param>
        /// <param></param>
        /// <returns>A list of seats in the hall.</returns>
        [HttpGet("{hallId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeatsByHallId(int hallId, CancellationToken cancellationToken)
        {
            var seats = await _seatService.GetSeatsByHallIdAsync(hallId, cancellationToken);
            return Ok(seats);
        }

        /// <summary>
        /// Adds a seat to a cinema hall. Only Admins and SuperAdmins can add seats.
        /// </summary>
        /// <param name="createSeatDTO">The details of the new seat.</param>
        /// <param></param>
        /// <returns>HTTP 201 Created with the new seat.</returns>
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

        /// <summary>
        /// Updates a seat by ID. Only Admins and SuperAdmins can update seats.
        /// </summary>
        /// <param name="id">The ID of the seat to update.</param>
        /// <param name="updateSeatDTO">The updated seat details.</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateSeat(int id, UpdateSeatDTO updateSeatDTO, CancellationToken cancellationToken)
        {
            await _seatService.UpdateSeatAsync(id, updateSeatDTO, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a seat by ID. Only Admins and SuperAdmins can delete seats.
        /// </summary>
        /// <param name="id">The ID of the seat to delete.</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteSeat(int id, CancellationToken cancellationToken)
        {
            await _seatService.DeleteSeatAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Gets the available seats for a session.
        /// </summary>
        /// <param name="sessionId">The ID of the session.</param>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("{sessionId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableSeatsBySessionId(int sessionId, CancellationToken cancellationToken)
        {
            var availableSeats = await _seatService.GetAvaliableSeatsBySessionIdAsync(sessionId, cancellationToken);
            return Ok(availableSeats);
        }
    }
}
