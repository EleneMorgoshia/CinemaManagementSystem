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

        /// <summary>
        /// Returns all cinema halls. Anyone can view them.
        /// </summary>
        /// <param></param>
        /// <returns>A list of all cinema halls</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllHalls(CancellationToken cancellationToken)
        {
            var halls = await _hallService.GetAllHallsAsync(cancellationToken);
            return Ok(halls);
        }

        /// <summary>
        ///  Returns a cinema hall by ID. Anyone can view it.
        /// </summary>
        /// <param name="id">The ID of the hall to retrieve</param>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHallById(int id, CancellationToken cancellationToken)
        {
            var hall = await _hallService.GetHallByIdAsync(id, cancellationToken);
            return Ok(hall);
        }

        /// <summary>
        /// Adds a new cinema hall. Only Admins and SuperAdmins can add halls.
        /// </summary>
        /// <param name="createHallDTO">The details of the new hall</param>
        /// <param name="cancellationToken"></param>
        /// <returns>HTTP 201 Created with the new hall</returns>

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
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

        /// <summary>
        /// Updates a cinema hall by ID. Only Admins and SuperAdmins can update halls.
        /// </summary>
        /// <param name="id">The ID of the hall to update</param>
        /// <param name="updateHallDTO">The updated hall details</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateHall(int id, UpdateHallDTO updateHallDTO, CancellationToken cancellationToken)
        {
            await _hallService.UpdateHallAsync(id, updateHallDTO, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a cinema hall by ID. Only Admins and SuperAdmins can delete halls.
        /// </summary>
        /// <param name="id">The ID of the hall to delete</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteHall(int id, CancellationToken cancellationToken)
        {
            await _hallService.DeleteHallAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
