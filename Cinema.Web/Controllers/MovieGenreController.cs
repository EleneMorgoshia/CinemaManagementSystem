using CinemaApplication.Services.Implementations;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MovieGenreController : ControllerBase
    {
        private readonly IMovieGenreService _movieGenreService;
        public MovieGenreController(IMovieGenreService movieGenreService)
        {
            _movieGenreService = movieGenreService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllMovieGenres(CancellationToken cancellationToken)
        {
            var movieGenres = await _movieGenreService.GetAllMovieGenresAsync(cancellationToken);
            return Ok(movieGenres);
        }


        [HttpGet]
        public async Task<IActionResult> GetGenresByMovieId(int movieId, CancellationToken cancellationToken)
        {
            var genres = await _movieGenreService.GetGenresByMovieIdAsync(movieId, cancellationToken);
            return Ok(genres);
        }

        [HttpGet]
        public async Task<IActionResult> GetMoviesByGenreId(int genreId, CancellationToken cancellationToken)
        {
            var movies = await _movieGenreService.GetMoviesByGenreIdAsync(genreId, cancellationToken);
            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> AddGenreToMovie(int movieId, int genreId, CancellationToken cancellationToken)
        {
            var movieGenre = await _movieGenreService.AddGenreToMovieAsync(movieId, genreId, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, movieGenre);
        }

        [HttpDelete("{movieId:int}/{genreId:int}")]
        public async Task<IActionResult> RemoveGenreFromMovieAsync(int movieId, int genreId, CancellationToken cancellationToken)
        {
            await  _movieGenreService.RemoveGenreFromMovieAsync(movieId, genreId, cancellationToken);
            return NoContent();
        }
    }
}
