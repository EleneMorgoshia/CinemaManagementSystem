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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTickets(CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetAllTicketsAsync(cancellationToken);
            return Ok(tickets);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetTicketById(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ClaimTypes - მიმდინარე მომხმარებლის ID-ს ვიღებთ JWT ტოკენიდან(ტოკენში ინახება მიმდინარე მომხმარებლის აიდი)

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            var ticket = await _ticketService.GetTicketByIdAsync(id, userId, isAdmin, cancellationToken);
            return Ok(ticket);
        }

        [HttpGet("{ticketCode}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetTicketByCode(string ticketCode, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ClaimTypes - მიმდინარე მომხმარებლის ID-ს ვიღებთ JWT ტოკენიდან(ტოკენში ინახება მიმდინარე მომხმარებლის აიდი)

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            
            var ticket = await _ticketService.GetTicketByCodeAsync(ticketCode, userId, isAdmin, cancellationToken);
            return Ok(ticket);
        }

        [HttpGet("{bookingId:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetTicketByBookingId(int bookingId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //ClaimTypes - მიმდინარე მომხმარებლის ID-ს ვიღებთ JWT ტოკენიდან(ტოკენში ინახება მიმდინარე მომხმარებლის აიდი)

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            var ticket = await _ticketService.GetTicketByBookingIdAsync(bookingId, userId, isAdmin, cancellationToken);
            return Ok(ticket);
        }
       
    }
}
