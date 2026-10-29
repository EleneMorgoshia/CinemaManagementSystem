using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetAllGenresAsync(cancellationToken);
            return Ok(genres);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGenreById(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreService.GetGenreByIdAsync(id, cancellationToken);
            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(CreateGenreDTO createGenreDTO, CancellationToken cancellationToken)
        {
            if(createGenreDTO == null)
                return BadRequest("Invalid data");
            var genre = await _genreService.AddGenreAsync(createGenreDTO, cancellationToken);
            return CreatedAtAction
                (
                    nameof(GetGenreById),
                    new { id = genre.Id },
                    genre
                );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDTO updateGenreDTO, CancellationToken cancellationToken)
        {
            await _genreService.UpdateGenreAsync(id, updateGenreDTO, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGenre(int id, CancellationToken cancellationToken)
        {
            await _genreService.DeleteGenreAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("{movieName}")]
        public async Task<IActionResult> GetGenresByMovieName(string movieName, CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetGenresByMovieNameAsync(movieName, cancellationToken);
            return Ok(genres);
        }
    }
}


