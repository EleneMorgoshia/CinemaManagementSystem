using AutoMapper;
using CinemaApplication.Services.Implementations;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using CinemaDomain.Models.DTOs;
using CinemaDomain.Models.Entities;
using CinemaInfrastructure.Identity;
using Microsoft.AspNetCore.Identity;
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
        [Fact]
        public async Task AddBookingAsync_AlreadyBookedSeat_DoesNotCallAddBooking()
        {
            // Arrange
            var bookingRepoMock = new Mock<IBookingRepository>();
            var sessionRepoMock = new Mock<ISessionRepository>();
            var seatRepoMock = new Mock<ISeatRepository>();
            var ticketServiceMock = new Mock<ITicketService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!,
                null!, null!, null!, null!);

            var dto = new CreateBookingDTO
            {
                SessionId = 20,
                SeatId = 10
            };

            userManagerMock
                .Setup(u => u.FindByIdAsync("user-1"))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = "user-1"
                });

            sessionRepoMock
                .Setup(r => r.GetSessionById(
                    20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Session
                {
                    Id = 20,
                    HallId = 1,
                    StartsAt = DateTime.UtcNow.AddDays(1),
                    Status = true
                });

            seatRepoMock
                .Setup(r => r.GetSeatById(
                    10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Seat
                {
                    Id = 10,
                    HallId = 1
                });

            // This seat already has an active booking.
            bookingRepoMock
                .Setup(r => r.IsSeatBookedForSession(
                    10, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var bookingService = new BookingService(
                bookingRepoMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object,
                userManagerMock.Object,
                seatRepoMock.Object,
                sessionRepoMock.Object,
                ticketServiceMock.Object);

            // Act
            var exception =
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => bookingService.AddBookingAsync(
                        dto,
                        "user-1",
                        CancellationToken.None));

            // Assert
            Assert.Equal(
                "This seat is already booked.",
                exception.Message);

            bookingRepoMock.Verify(
                r => r.AddBooking(
                    It.IsAny<Booking>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData("another-user", true, false, "owner")]
        [InlineData("user-1", false, false, "cancelled")]
        [InlineData("user-1", true, true, "started")]
        public async Task UpdateBookingAsync_InvalidCancellation_DoesNotSave( string bookingOwnerId, bool bookingStatus,bool sessionStarted,string scenario)
        {
            // Arrange
            var bookingRepoMock = new Mock<IBookingRepository>();
            var sessionRepoMock = new Mock<ISessionRepository>();
            var seatRepoMock = new Mock<ISeatRepository>();
            var ticketServiceMock = new Mock<ITicketService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!,
                null!, null!, null!, null!);

            var booking = new Booking
            {
                Id = 1,
                UserId = bookingOwnerId,
                SessionId = 20,
                Status = bookingStatus
            };

            bookingRepoMock
                .Setup(r => r.GetBookingById(
                    1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            userManagerMock
                .Setup(u => u.FindByIdAsync("user-1"))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = "user-1"
                });

            sessionRepoMock
                .Setup(r => r.GetSessionById(
                    20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Session
                {
                    Id = 20,
                    StartsAt = sessionStarted
                        ? DateTime.UtcNow.AddDays(-1)
                        : DateTime.UtcNow.AddDays(1)
                });

            var service = new BookingService(
                bookingRepoMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object,
                userManagerMock.Object,
                seatRepoMock.Object,
                sessionRepoMock.Object,
                ticketServiceMock.Object);

            var dto = new CancelBookingDTO { Id = 1 };

            // Act and Assert
            if (scenario == "owner")
            {
                var exception =
                    await Assert.ThrowsAsync<UnauthorizedAccessException>(
                        () => service.UpdateBookingAsync(
                            1, dto, "user-1", CancellationToken.None));

                Assert.Equal(
                    "You cannot cancel another user's booking.",
                    exception.Message);
            }
            else
            {
                var exception =
                    await Assert.ThrowsAsync<InvalidOperationException>(
                        () => service.UpdateBookingAsync(
                            1, dto, "user-1", CancellationToken.None));

                var expectedMessage = scenario == "cancelled"
                    ? "Booking is already cancelled."
                    : "You cannot cancel a booking after the session has started.";

                Assert.Equal(expectedMessage, exception.Message);
            }

            bookingRepoMock.Verify(
                r => r.UpdateBooking(It.IsAny<Booking>()),
                Times.Never);

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task UpdateBookingAsync_ValidCancellation_UpdatesAndSaves()
        {
            // Arrange
            var bookingRepoMock = new Mock<IBookingRepository>();
            var sessionRepoMock = new Mock<ISessionRepository>();
            var seatRepoMock = new Mock<ISeatRepository>();
            var ticketServiceMock = new Mock<ITicketService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!,
                null!, null!, null!, null!);

            var booking = new Booking
            {
                Id = 1,
                UserId = "user-1",
                SessionId = 20,
                Status = true
            };

            bookingRepoMock
                .Setup(r => r.GetBookingById(
                    1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            userManagerMock
                .Setup(u => u.FindByIdAsync("user-1"))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = "user-1"
                });

            sessionRepoMock
                .Setup(r => r.GetSessionById(
                    20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Session
                {
                    Id = 20,
                    StartsAt = DateTime.UtcNow.AddDays(1)
                });

            var service = new BookingService(
                bookingRepoMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object,
                userManagerMock.Object,
                seatRepoMock.Object,
                sessionRepoMock.Object,
                ticketServiceMock.Object);

            var dto = new CancelBookingDTO { Id = 1 };

            // Act
            await service.UpdateBookingAsync(
                1, dto, "user-1", CancellationToken.None);

            // Assert: the booking becomes cancelled.
            Assert.False(booking.Status);

            bookingRepoMock.Verify(
                r => r.UpdateBooking(booking),
                Times.Once);

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task AddBookingAsync_SeatFromAnotherHall_DoesNotSave()
        {
            // Arrange
            var bookingRepoMock = new Mock<IBookingRepository>();
            var sessionRepoMock = new Mock<ISessionRepository>();
            var seatRepoMock = new Mock<ISeatRepository>();
            var ticketServiceMock = new Mock<ITicketService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!,
                null!, null!, null!, null!);

            var dto = new CreateBookingDTO
            {
                SessionId = 20,
                SeatId = 10
            };

            userManagerMock
                .Setup(u => u.FindByIdAsync("user-1"))
                .ReturnsAsync(new ApplicationUser { Id = "user-1" });

            sessionRepoMock
                .Setup(r => r.GetSessionById(
                    20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Session
                {
                    Id = 20,
                    HallId = 1,
                    StartsAt = DateTime.UtcNow.AddDays(1),
                    Status = true
                });

            // The seat belongs to hall 2, but the session is in hall 1.
            seatRepoMock
                .Setup(r => r.GetSeatById(
                    10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Seat
                {
                    Id = 10,
                    HallId = 2
                });

            var service = new BookingService(
                bookingRepoMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object,
                userManagerMock.Object,
                seatRepoMock.Object,
                sessionRepoMock.Object,
                ticketServiceMock.Object);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.AddBookingAsync(
                    dto, "user-1", CancellationToken.None));

            // Assert
            Assert.Equal(
                "Seat does not belong to the session's hall.",
                exception.Message);

            bookingRepoMock.Verify(
                r => r.AddBooking(
                    It.IsAny<Booking>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            ticketServiceMock.Verify(
                t => t.AddTicketAsync(
                    It.IsAny<Booking>(),
                    It.IsAny<decimal>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData(false, 15)] // Standard seat -0 15.
        [InlineData(true, 25)]  // VIP seat - 25.
        public async Task AddBookingAsync_ValidBooking_CreatesTicketWithCorrectPrice(bool isVip,int expectedPrice)
        {
            // Arrange
            var bookingRepoMock = new Mock<IBookingRepository>();
            var sessionRepoMock = new Mock<ISessionRepository>();
            var seatRepoMock = new Mock<ISeatRepository>();
            var ticketServiceMock = new Mock<ITicketService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!,
                null!, null!, null!, null!);

            var dto = new CreateBookingDTO
            {
                SessionId = 20,
                SeatId = 10
            };

            var booking = new Booking
            {
                SessionId = 20,
                SeatId = 10
            };

            var response = new BookingDTO();

            userManagerMock
                .Setup(u => u.FindByIdAsync("user-1"))
                .ReturnsAsync(new ApplicationUser { Id = "user-1" });

            sessionRepoMock
                .Setup(r => r.GetSessionById(
                    20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Session
                {
                    Id = 20,
                    HallId = 1,
                    StartsAt = DateTime.UtcNow.AddDays(1),
                    Status = true,
                    StandardPrice = 15m,
                    VipPrice = 25m
                });

            seatRepoMock
                .Setup(r => r.GetSeatById(
                    10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Seat
                {
                    Id = 10,
                    HallId = 1,
                    IsVip = isVip
                });

            bookingRepoMock
                .Setup(r => r.IsSeatBookedForSession(
                    10, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mapperMock
                .Setup(m => m.Map<CreateBookingDTO, Booking>(dto))
                .Returns(booking);

            mapperMock
                .Setup(m => m.Map<BookingDTO>(booking))
                .Returns(response);

            bookingRepoMock
                .Setup(r => r.AddBooking(
                    booking, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ticketServiceMock
                .Setup(t => t.AddTicketAsync(
                    booking,
                    (decimal)expectedPrice,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new BookingService(
                bookingRepoMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object,
                userManagerMock.Object,
                seatRepoMock.Object,
                sessionRepoMock.Object,
                ticketServiceMock.Object);

            var beforeBooking = DateTime.UtcNow;

            // Act
            var result = await service.AddBookingAsync(
                dto, "user-1", CancellationToken.None);

            var afterBooking = DateTime.UtcNow;

            // Assert
            Assert.Same(response, result);
            Assert.Equal("user-1", booking.UserId);
            Assert.True(booking.Status);
            Assert.InRange(booking.CreatedAt, beforeBooking, afterBooking);

            bookingRepoMock.Verify(
                r => r.AddBooking(booking, CancellationToken.None),
                Times.Once);

            ticketServiceMock.Verify(
                t => t.AddTicketAsync(
                    booking,
                    (decimal)expectedPrice,
                    CancellationToken.None),
                Times.Once);

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(CancellationToken.None),
                Times.Once);
        }
    }
}
