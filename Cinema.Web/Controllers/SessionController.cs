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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetSessionsAsync(cancellationToken);
            return Ok(sessions);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionById(int id, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetSessionByIdAsync(id, cancellationToken);
            return Ok(session);
        }

        [HttpGet("{movieId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionsByMovieId(int movieId, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetSessionsByMovieIdAsync(movieId, cancellationToken);
            return Ok(session);
        }

        [HttpGet("{hallId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionsByHallId(int hallId, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetSessionsByHallIdAsync(hallId, cancellationToken);
            return Ok(session);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSession(int id, UpdateSessionDTO updateSessionDTO, CancellationToken cancellationToken)
        {
            await _sessionService.UpdateSession(id, updateSessionDTO, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSession(int id, CancellationToken cancellationToken)
        {
            await _sessionService.DeleteSession(id, cancellationToken);
            return NoContent();
        }
    }
}
