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

        /// <summary>
        /// Returns all genres. Anyone can view them.
        /// </summary>
        /// <param ></param>
        /// <returns>A list of all genres</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetAllGenresAsync(cancellationToken);
            return Ok(genres);
        }

        /// <summary>
        /// Returns a genre by ID. Anyone can view it.
        /// </summary>
        /// <param name="id">The ID of the genre to retrieve</param>
        /// <param ></param>
        /// <returns>The requested genre</returns>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGenreById(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreService.GetGenreByIdAsync(id, cancellationToken);
            return Ok(genre);
        }

        /// <summary>
        /// Adds a new genre. Only Admins and SuperAdmins can add genres.
        /// </summary>
        /// <param name="createGenreDTO">The details of the new genre</param>
        /// <param ></param>
        /// <returns>HTTP 201 Created with the new genre</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
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

        /// <summary>
        /// Updates a genre by ID. Only Admins and SuperAdmins can update genres.
        /// </summary>
        /// <param name="id">The ID of the genre to update</param>
        /// <param name="updateGenreDTO">The updated genre details</param>
        /// <param ></param>
        /// <returns>HTTP 204 No Content on success</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDTO updateGenreDTO, CancellationToken cancellationToken)
        {
            await _genreService.UpdateGenreAsync(id, updateGenreDTO, cancellationToken);
            return NoContent();
        }

        /// <summary>
        ///  Deletes a genre by ID. Only Admins and SuperAdmins can delete genres.
        /// </summary>
        /// <param name="id">The ID of the genre to delete</param>
        /// <param ></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteGenre(int id, CancellationToken cancellationToken)
        {
            await _genreService.DeleteGenreAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Finds a movie's genres by its name. Anyone can use this endpoint.
        /// </summary>
        /// <param name="movieName">The name of the movie</param>
        /// <param></param>
        /// <returns>A list of genres associated with the movie</returns>
        [HttpGet("{movieName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGenresByMovieName(string movieName, CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetGenresByMovieNameAsync(movieName, cancellationToken);
            return Ok(genres);
        }
    }
}


