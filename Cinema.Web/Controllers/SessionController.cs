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
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        public SessionController(ISessionService sessionService) 
        { 
            _sessionService = sessionService;
        }

        /// <summary>
        /// Returns all cinema sessions. Anyone can view them.
        /// </summary>
        /// <param></param>
        /// <returns>A list of all sessions.</returns>

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetSessionsAsync(cancellationToken);
            return Ok(sessions);
        }

        /// <summary>
        /// Returns sessions that are open for booking.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveSessions(CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetActiveSessionsAsync(cancellationToken);
            return Ok(sessions);
        }

        /// <summary>
        /// Returns a cinema session by ID. Anyone can view it.
        /// </summary>
        /// <param name="id">The ID of the session to retrieve.</param>
        /// <param></param>
        /// <returns>The requested session.</returns>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionById(int id, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetSessionByIdAsync(id, cancellationToken);
            return Ok(session);
        }

        /// <summary>
        /// Returns the sessions for a movie. Anyone can view them.
        /// </summary>
        /// <param name="movieId">The ID of the movie.</param>
        /// <param></param>
        /// <returns>A list of sessions for the movie.</returns>
        [HttpGet("{movieId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionsByMovieId(int movieId, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetSessionsByMovieIdAsync(movieId, cancellationToken);
            return Ok(session);
        }

        /// <summary>
        /// Returns all sessions for the specified hall ID. Anyone can view them.
        /// </summary>
        /// <param name="hallId">The ID of the hall.</param>
        /// <param></param>
        /// <returns>A list of sessions in the hall.</returns>
        [HttpGet("{hallId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionsByHallId(int hallId, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetSessionsByHallIdAsync(hallId, cancellationToken);
            return Ok(session);
        }

        /// <summary>
        /// Schedules a new session. Only Admins and SuperAdmins can do this.
        /// </summary>
        /// <param name="createSessionDto">The details of the new session.</param>
        /// <param></param>
        /// <returns>HTTP 201 Created with the new session.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> AddSession(CreateSessionDTO createSessionDto, CancellationToken cancellationToken)
        {
            var session = await _sessionService.AddSessionAsync(createSessionDto, cancellationToken);
            return CreatedAtAction
                (
                    nameof(GetSessionById),
                    new { id = session.Id },
                    session
                );
        }

        /// <summary>
        /// Updates a session by ID. Only Admins and SuperAdmins can update sessions.
        /// </summary>
        /// <param name="id">The ID of the session to update.</param>
        /// <param name="updateSessionDTO">The updated session details.</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateSession(int id, UpdateSessionDTO updateSessionDTO, CancellationToken cancellationToken)
        {
            await _sessionService.UpdateSession(id, updateSessionDTO, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a session by ID. Only Admins and SuperAdmins can delete sessions.
        /// </summary>
        /// <param name="id">The ID of the session to delete.</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteSession(int id, CancellationToken cancellationToken)
        {
            await _sessionService.DeleteSession(id, cancellationToken);
            return NoContent();
        }
    }
}
