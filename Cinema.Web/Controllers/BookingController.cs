using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cinema.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Returns all bookings. Only Admins and SuperAdmins can access this endpoint.
        /// </summary>
        /// <param ></param>
        /// <returns>A list of all bookings</returns>

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllBookings(CancellationToken cancellationToken)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(cancellationToken);
            return Ok(bookings);
        }

        /// <summary>
        /// Returns a booking by ID. The owner, Admins, and SuperAdmins can view it.
        /// </summary>
        /// <param name="id">The ID of the booking to retrieve</param>
        /// <param ></param>
        /// <returns>The requested booking</returns>
        [HttpGet]
        public async Task<IActionResult> GetBookingById(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ClaimTypes - მიმდინარე მომხმარებლის ID-ს ვიღებთ JWT ტოკენიდან(ტოკენში ინახება მიმდინარე მომხმარებლის აიდი)

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var booking = await _bookingService.GetBookingByIdAsync(id, userId, isAdmin, cancellationToken);

            return Ok(booking);
        }

        /// <summary>
        /// Returns a user's booking history. Users can view their own history, and Admins and SuperAdmins can view anyone's history.
        /// </summary>
        /// <param name="userId">he ID of the user whose bookings to retrieve</param>
        /// <param ></param>
        /// <returns>The ID of the user whose bookings to retrieve</returns>
        [HttpGet]
        public async Task<IActionResult> GetBookingsByUserId(string userId, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserId))
                return Unauthorized();

            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && userId != currentUserId)
                return Forbid();

            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId, cancellationToken);
            return Ok(bookings);
        }

        /// <summary>
        /// Returns all bookings for a session. Only Admins and SuperAdmins can access this endpoint.
        /// </summary>
        /// <param name="sessionId">The ID of the session whose bookings to retrieve</param>
        /// <param ></param>
        /// <returns>A list of bookings for the session</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetBookingsBySessionId(int sessionId, CancellationToken cancellationToken)
        {
            var bookings = await _bookingService.GetBookingsBySessionIdAsync(sessionId, cancellationToken);
            return Ok(bookings);
        }

        /// <summary>
        /// Books a seat for the signed-in user and returns the new booking.
        /// </summary>
        /// <param name="creatingBookingDTO">The session and seat IDs for the new booking</param>
        /// <param ></param>
        /// <returns>HTTP 201 Created with the new booking</returns>
        [HttpPost]
        public async Task<IActionResult> AddBooking(CreateBookingDTO creatingBookingDTO, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ტოკენიდან ვიღებ აიდს იზუერის
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var booking = await _bookingService.AddBookingAsync(creatingBookingDTO, userId, cancellationToken);
            return CreatedAtAction
                (
                    nameof(GetBookingById),
                    new { id = booking.Id },
                    booking
                );
        }

        /// <summary>
        /// Cancels a booking and keeps it in the user's booking history.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to cancel</param>
        /// <param name="cancelBookingDTO">The cancellation details</param>
        /// <param ></param>
        /// <returns>HTTP 204 No Content on success</returns>
        [HttpPut("{bookingId:int}")]
        //დაქენსელებულების გეთი არ მინდა იმიტომ რომ getBookings ში ჩანს ყველანაირი ბუქინგი. მერე ფრონტენდერმა
        //ბუქინგის სტატუსის მიხედვით გამოიტანოს დაქენსელებულები ცალკე აქტიურები ცალკე
        public async Task<IActionResult> CancelBooking(int bookingId, CancelBookingDTO cancelBookingDTO, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ტოკენიდან ვიღებ აიდს იზუერის
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            
            await _bookingService.UpdateBookingAsync(bookingId, cancelBookingDTO, userId, cancellationToken);
            return NoContent();
        }


        /// <summary>
        /// Deletes a booking by ID. Only Admins and SuperAdmins can delete bookings.
        /// </summary>
        /// <param name="id">The ID of the booking to delete</param>
        /// <param></param>
        /// <returns>HTTP 204 No Content on success</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeleteBooking(int id,CancellationToken cancellationToken)
        {
            await _bookingService.DeleteBookingAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
