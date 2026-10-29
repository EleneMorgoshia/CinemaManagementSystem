using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]/[action]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllMovies(CancellationToken cancellationToken)
        {
            var movies = await _movieService.GetAllMoviesAsync(cancellationToken);
            return Ok(movies);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMovie(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieService.GetMovieByIdAsync(id, cancellationToken);
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(CreateMovieDTO movieDto, CancellationToken cancellationToken)
        {
            if (movieDto == null)
                return BadRequest("Invalid data");
            var movie = await _movieService.AddMovieAsync(movieDto, cancellationToken);
            return CreatedAtAction(
               nameof(GetMovie),
               new { id = movie.Id },
               movie
           );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMovie(int id, UpdateMovieDTO updateMovieDTO, CancellationToken cancellationToken)
        {
            await _movieService.UpdateMovieAsync(id, updateMovieDTO, cancellationToken);
            return NoContent();
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMovie(int id, CancellationToken cancellationToken)
        {
            await _movieService.DeleteMovieAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
