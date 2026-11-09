using CinemaApplication.Services.Implementations;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MovieGenreController : ControllerBase
    {
        private readonly IMovieGenreService _movieGenreService;
        public MovieGenreController(IMovieGenreService movieGenreService)
        {
            _movieGenreService = movieGenreService;
        }

        /// <summary>
        /// Returns all movie and genre pairs. Anyone can view them.
        /// </summary>
        /// <param ></param>
        /// <returns>A list of all movie and genre pairs.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllMovieGenres(CancellationToken cancellationToken)
        {
            var movieGenres = await _movieGenreService.GetAllMovieGenresAsync(cancellationToken);
            return Ok(movieGenres);
        }

        /// <summary>
        /// Returns the genres assigned to a movie. Anyone can view them.
        /// </summary>
        /// <param name="movieId">The ID of the movie.</param>
        /// <param ></param>
        /// <returns>A list of genres for the movie.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetGenresByMovieId(int movieId, CancellationToken cancellationToken)
        {
            var genres = await _movieGenreService.GetGenresByMovieIdAsync(movieId, cancellationToken);
            return Ok(genres);
        }

        /// <summary>
        /// Returns the movies in a genre. Anyone can view them.
        /// </summary>
        /// <param name="genreId">The ID of the genre.</param>
        /// <param></param>
        /// <returns>A list of movies in the genre.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetMoviesByGenreId(int genreId, CancellationToken cancellationToken)
        {
            var movies = await _movieGenreService.GetMoviesByGenreIdAsync(genreId, cancellationToken);
            return Ok(movies);
        }

        /// <summary>
        /// Assigns a genre to a movie. Only Admins and SuperAdmins can do this.
        /// </summary>
        /// <param name="movieId">The ID of the movie</param>
        /// <param name="genreId">The ID of the genre to assign</param>
        /// <param ></param>
        /// <returns>HTTP 201 Created with the new movie and genre pair.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> AddGenreToMovie(int movieId, int genreId, CancellationToken cancellationToken)
        {
            var movieGenre = await _movieGenreService.AddGenreToMovieAsync(movieId, genreId, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, movieGenre);
        }

        /// <summary>
        /// Removes a genre from a movie. Only Admins and SuperAdmins can do this.
        /// </summary>
        /// <param name="movieId">The ID of the movie.</param>
        /// <param name="genreId">The ID of the genre to remove from the movie.</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpDelete("{movieId:int}/{genreId:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> RemoveGenreFromMovieAsync(int movieId, int genreId, CancellationToken cancellationToken)
        {
            await  _movieGenreService.RemoveGenreFromMovieAsync(movieId, genreId, cancellationToken);
            return NoContent();
        }
    }
}
