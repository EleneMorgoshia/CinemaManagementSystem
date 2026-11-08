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

        
        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllBookings(CancellationToken cancellationToken)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(cancellationToken);
            return Ok(bookings);
        }

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

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetBookingsBySessionId(int sessionId, CancellationToken cancellationToken)
        {
            var bookings = await _bookingService.GetBookingsBySessionIdAsync(sessionId, cancellationToken);
            return Ok(bookings);
        }

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

     
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeleteBooking(int id,CancellationToken cancellationToken)
        {
            await _bookingService.DeleteBookingAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
