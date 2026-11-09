using CinemaApplication.Services.Interfaces;
using CinemaDomain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cinema.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Returns all tickets. Only Admins and SuperAdmins can view them.
        /// </summary>
        /// <param></param>
        /// <returns>A list of all tickets.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllTickets(CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetAllTicketsAsync(cancellationToken);
            return Ok(tickets);
        }

        /// <summary>
        /// Returns a ticket by its ID. The owner, Admins, and SuperAdmins can view it.
        /// </summary>
        /// <param name="id">The ID of the ticket to retrieve.</param>
        /// <param></param>
        /// <returns>The requested ticket.</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicketById(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ClaimTypes - მიმდინარე მომხმარებლის ID-ს ვიღებთ JWT ტოკენიდან(ტოკენში ინახება მიმდინარე მომხმარებლის აიდი)

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var ticket = await _ticketService.GetTicketByIdAsync(id, userId, isAdmin, cancellationToken);
            return Ok(ticket);
        }

        /// <summary>
        /// Returns a ticket by its code. The owner, Admins, and SuperAdmins can view it.
        /// </summary>
        /// <param name="ticketCode">The unique code of the ticket.</param>
        /// <param></param>
        /// <returns>The ticket with the specified code.</returns>
        [HttpGet("{ticketCode}")]
        public async Task<IActionResult> GetTicketByCode(string ticketCode, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ClaimTypes - მიმდინარე მომხმარებლის ID-ს ვიღებთ JWT ტოკენიდან(ტოკენში ინახება მიმდინარე მომხმარებლის აიდი)

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var ticket = await _ticketService.GetTicketByCodeAsync(ticketCode, userId, isAdmin, cancellationToken);
            return Ok(ticket);
        }

        /// <summary>
        /// Returns a ticket by its booking ID. The owner, Admins, and SuperAdmins can view it.
        /// </summary>
        /// <param name="bookingId">The ID of the booking linked to the ticket.</param>
        /// <param></param>
        /// <returns>The ticket for the specified booking.</returns>
        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetTicketByBookingId(int bookingId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ClaimTypes - მიმდინარე მომხმარებლის ID-ს ვიღებთ JWT ტოკენიდან(ტოკენში ინახება მიმდინარე მომხმარებლის აიდი)

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var ticket = await _ticketService.GetTicketByBookingIdAsync(bookingId, userId, isAdmin, cancellationToken);
            return Ok(ticket);
        }
       
    }
}
