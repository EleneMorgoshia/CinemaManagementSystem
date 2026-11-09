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

        /// <summary>
        /// Returns all movies. Anyone can view them.
        /// </summary>
        /// <param></param>
        /// <returns>A list of all movies</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllMovies(CancellationToken cancellationToken)
        {
            var movies = await _movieService.GetAllMoviesAsync(cancellationToken);
            return Ok(movies);
        }

        /// <summary>
        /// Returns a movie by ID. Anyone can view it.
        /// </summary>
        /// <param name="id">The ID of the movie to retrieve.</param>
        /// <param></param>
        /// <returns>The requested movie.</returns>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMovie(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieService.GetMovieByIdAsync(id, cancellationToken);
            return Ok(movie);
        }

        /// <summary>
        /// Adds a new movie. Only Admins and SuperAdmins can add movies.
        /// </summary>
        /// <param name="movieDto">The details of the new movie.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>HTTP 201 Created with the new movie.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
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

        /// <summary>
        /// Updates a movie by ID. Only Admins and SuperAdmins can update movies.
        /// </summary>
        /// <param name="id">The ID of the movie to update.</param>
        /// <param name="updateMovieDTO">The updated movie details.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateMovie(int id, UpdateMovieDTO updateMovieDTO, CancellationToken cancellationToken)
        {
            await _movieService.UpdateMovieAsync(id, updateMovieDTO, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a movie by ID. Only Admins and SuperAdmins can delete movies.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteMovie(int id, CancellationToken cancellationToken)
        {
            await _movieService.DeleteMovieAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
