using AutoMapper;
using CinemaApplication.Services.Implementations;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using CinemaInfrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Cinema.Tests.Services
{
    public class BookingServiceTest
    {
        private const string UserId = "user-1";
        private const int BookingId = 1;
        private const int SeatId = 10;
        private const int SessionId = 20;
        private const int HallId = 1;

        private readonly Mock<IBookingRepository> _bookingRepository = new();
        private readonly Mock<ISessionRepository> _sessionRepository = new();
        private readonly Mock<ISeatRepository> _seatRepository = new();
        private readonly Mock<ITicketService> _ticketService = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly BookingService _service;

        public BookingServiceTest()
        {
            var userManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!,
                null!, null!, null!, null!);

            userManager
                .Setup(manager => manager.FindByIdAsync(UserId))
                .ReturnsAsync(new ApplicationUser { Id = UserId });

            _service = new BookingService(
                _bookingRepository.Object,
                _unitOfWork.Object,
                _mapper.Object,
                userManager.Object,
                _seatRepository.Object,
                _sessionRepository.Object,
                _ticketService.Object,
                Mock.Of<IEmailService>(),
                NullLogger<BookingService>.Instance);
        }

        [Fact]
        public async Task AddBookingAsync_AlreadyBookedSeat_DoesNotAddBooking()
        {
            // Arrange
            var dto = new CreateBookingDTO
            {
                SessionId = SessionId,
                SeatId = SeatId
            };

            SetUpSession(DateTime.UtcNow.AddDays(1));
            SetUpSeat(HallId, isVip: false);

            _bookingRepository
                .Setup(repository => repository.IsSeatBookedForSession(
                    SeatId, SessionId, CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddBookingAsync(dto, UserId, CancellationToken.None));

            // Assert
            Assert.Equal("This seat is already booked.", exception.Message);

            _bookingRepository.Verify(
                repository => repository.AddBooking(
                    It.IsAny<Booking>(), It.IsAny<CancellationToken>()),
                Times.Never());

            _unitOfWork.Verify(
                unitOfWork => unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public async Task AddBookingAsync_SeatFromAnotherHall_DoesNotSave()
        {
            // Arrange
            var dto = new CreateBookingDTO
            {
                SessionId = SessionId,
                SeatId = SeatId
            };

            SetUpSession(DateTime.UtcNow.AddDays(1));
            SetUpSeat(hallId: 2, isVip: false);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddBookingAsync(dto, UserId, CancellationToken.None));

            // Assert
            Assert.Equal("Seat does not belong to the session's hall.", exception.Message);

            _bookingRepository.Verify(
                repository => repository.AddBooking(
                    It.IsAny<Booking>(), It.IsAny<CancellationToken>()),
                Times.Never());

            _ticketService.Verify(
                service => service.AddTicketAsync(
                    It.IsAny<Booking>(),
                    It.IsAny<decimal>(),
                    It.IsAny<CancellationToken>()),
                Times.Never());

            _unitOfWork.Verify(
                unitOfWork => unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Theory]
        [InlineData(false, 15)]
        [InlineData(true, 25)]
        public async Task AddBookingAsync_ValidBooking_CreatesTicketWithCorrectPrice(
            bool isVip,
            int expectedPrice)
        {
            // Arrange
            var dto = new CreateBookingDTO
            {
                SessionId = SessionId,
                SeatId = SeatId
            };

            var booking = new Booking
            {
                SessionId = SessionId,
                SeatId = SeatId
            };

            var response = new BookingDTO();
            var finalPrice = (decimal)expectedPrice;

            SetUpSession(DateTime.UtcNow.AddDays(1));
            SetUpSeat(HallId, isVip);

            _bookingRepository
                .Setup(repository => repository.IsSeatBookedForSession(
                    SeatId, SessionId, CancellationToken.None))
                .ReturnsAsync(false);

            _mapper
                .Setup(mapper => mapper.Map<CreateBookingDTO, Booking>(dto))
                .Returns(booking);

            _mapper
                .Setup(mapper => mapper.Map<BookingDTO>(booking))
                .Returns(response);

            _bookingRepository
                .Setup(repository => repository.AddBooking(
                    booking, CancellationToken.None))
                .Returns(Task.CompletedTask);

            _ticketService
                .Setup(service => service.AddTicketAsync(
                    booking, finalPrice, CancellationToken.None))
                .ReturnsAsync(() => new()
                {
                    TicketCode = "TEST-001",
                    FinalPrice = finalPrice
                });

            var beforeBooking = DateTime.UtcNow;

            // Act
            var result = await _service.AddBookingAsync(
                dto, UserId, CancellationToken.None);

            // Assert
            Assert.Same(response, result);
            Assert.Equal(UserId, booking.UserId);
            Assert.True(booking.Status);
            Assert.InRange(booking.CreatedAt, beforeBooking, DateTime.UtcNow);

            _bookingRepository.Verify(
                repository => repository.AddBooking(
                    booking, CancellationToken.None),
                Times.Once());

            _ticketService.Verify(
                service => service.AddTicketAsync(
                    booking, finalPrice, CancellationToken.None),
                Times.Once());

            _unitOfWork.Verify(
                unitOfWork => unitOfWork.SaveChangesAsync(CancellationToken.None),
                Times.Once());
        }

        [Theory]
        [InlineData("another-user", true, 1, typeof(UnauthorizedAccessException),
            "You cannot cancel another user's booking.")]
        [InlineData(UserId, false, 1, typeof(InvalidOperationException),
            "Booking is already cancelled.")]
        [InlineData(UserId, true, -1, typeof(InvalidOperationException),
            "You cannot cancel a booking after the session has started.")]
        public async Task UpdateBookingAsync_InvalidCancellation_DoesNotSave(
            string bookingOwnerId,
            bool bookingStatus,
            int sessionStartsInDays,
            Type expectedExceptionType,
            string expectedMessage)
        {
            // Arrange
            var booking = SetUpBooking(bookingOwnerId, bookingStatus);
            SetUpSession(DateTime.UtcNow.AddDays(sessionStartsInDays));
            var dto = new CancelBookingDTO { Id = BookingId };

            // Act
            var exception = await Assert.ThrowsAsync(
                expectedExceptionType,
                () => _service.UpdateBookingAsync(
                    BookingId, dto, UserId, CancellationToken.None));

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Equal(bookingStatus, booking.Status);

            _bookingRepository.Verify(
                repository => repository.UpdateBooking(It.IsAny<Booking>()),
                Times.Never());

            _unitOfWork.Verify(
                unitOfWork => unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public async Task UpdateBookingAsync_ValidCancellation_UpdatesAndSaves()
        {
            // Arrange
            var booking = SetUpBooking(UserId, status: true);
            SetUpSession(DateTime.UtcNow.AddDays(1));
            var dto = new CancelBookingDTO { Id = BookingId };

            // Act
            await _service.UpdateBookingAsync(
                BookingId, dto, UserId, CancellationToken.None);

            // Assert
            Assert.False(booking.Status);

            _bookingRepository.Verify(
                repository => repository.UpdateBooking(booking),
                Times.Once());

            _unitOfWork.Verify(
                unitOfWork => unitOfWork.SaveChangesAsync(CancellationToken.None),
                Times.Once());
        }

        private void SetUpSession(DateTime startsAt)
        {
            _sessionRepository
                .Setup(repository => repository.GetSessionById(
                    SessionId, CancellationToken.None))
                .ReturnsAsync(new Session
                {
                    Id = SessionId,
                    HallId = HallId,
                    StartsAt = startsAt,
                    Status = true,
                    StandardPrice = 15m,
                    VipPrice = 25m
                });
        }

        private void SetUpSeat(int hallId, bool isVip)
        {
            _seatRepository
                .Setup(repository => repository.GetSeatById(
                    SeatId, CancellationToken.None))
                .ReturnsAsync(new Seat
                {
                    Id = SeatId,
                    HallId = hallId,
                    IsVip = isVip
                });
        }

        private Booking SetUpBooking(string ownerId, bool status)
        {
            var booking = new Booking
            {
                Id = BookingId,
                UserId = ownerId,
                SessionId = SessionId,
                Status = status
            };

            _bookingRepository
                .Setup(repository => repository.GetBookingById(
                    BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            return booking;
        }
    }
}
