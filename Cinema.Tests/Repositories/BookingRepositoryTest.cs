using CinemaDomain.Models.Entities;
using CinemaInfrastructure.Data;
using CinemaInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Tests.Repositories
{
    public class BookingRepositoryTest
    {
        private CinemaDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new CinemaDbContext(options);
        }
        [Fact]
        public async Task GetAllBookings_ReturnsAllBookings()
        {
            // Arrange
            using var context = CreateContext();

            var booking1 = new Booking
            {
                UserId = "user-1",
                SessionId = 1,
                SeatId = 1,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            var booking2 = new Booking
            {
                UserId = "user-2",
                SessionId = 1,
                SeatId = 2,
                Status = false,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.AddRange(booking1, booking2);
            await context.SaveChangesAsync();

            var repository = new BookingRepository(context);

            // Act
            var result = await repository.GetAllBookings(CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Contains(result,
                b => b.UserId == "user-1" && b.Status);

            Assert.Contains(result,
                b => b.UserId == "user-2" && !b.Status);
        }
        [Fact]
        public async Task GetBookingById_WhenBookingExists_ReturnsBooking()
        {
            // Arrange
            using var context = CreateContext();

            var booking = new Booking
            {
                UserId = "user-1",
                SessionId = 1,
                SeatId = 1,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var repository = new BookingRepository(context);

            // Act
            var result = await repository.GetBookingById(
                booking.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking.Id, result.Id);
            Assert.Equal("user-1", result.UserId);
        }

        [Fact]
        public async Task GetBookingById_WhenBookingDoesNotExist_ReturnsNull()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BookingRepository(context);

            // Act
            var result = await repository.GetBookingById(
                999, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetBookingsByUserId_ReturnsOnlyThatUsersBookings()
        {
            // Arrange
            using var context = CreateContext();

            context.Bookings.AddRange(
                new Booking
                {
                    UserId = "user-1",
                    SessionId = 1,
                    SeatId = 1,
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = "user-1",
                    SessionId = 2,
                    SeatId = 2,
                    Status = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = "user-2",
                    SessionId = 1,
                    SeatId = 3,
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            var repository = new BookingRepository(context);

            // Act
            var result = await repository.GetBookingsByUserId(
                "user-1", CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result,
                b => Assert.Equal("user-1", b.UserId));
        }
        [Fact]
        public async Task GetBookingsBySessionId_ReturnsOnlyThatSessionsBookings()
        {
            // Arrange
            using var context = CreateContext();

            context.Bookings.AddRange(
                new Booking
                {
                    UserId = "user-1",
                    SessionId = 1,
                    SeatId = 1,
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = "user-2",
                    SessionId = 2,
                    SeatId = 2,
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            var repository = new BookingRepository(context);

            // Act
            var result = await repository.GetBookingsBySessionId(
                1, CancellationToken.None);

            // Assert
            var booking = Assert.Single(result);
            Assert.Equal(1, booking.SessionId);
            Assert.Equal("user-1", booking.UserId);
        }

        // Runs the same test with different booking statuses, seats, and sessions.
        [Theory]
        [InlineData(true, 10, 20, true)]    // Active booking for the same seat and session - booked.
        [InlineData(false, 10, 20, false)]  // Cancelled booking - seat is available.
        [InlineData(true, 11, 20, false)]   // Different seat - available.
        [InlineData(true, 10, 21, false)]   // Different session - available.
        public async Task IsSeatBookedForSession_ReturnsExpectedResult(bool status, int requestedSeatId, int requestedSessionId, bool expected)
        {
            // Arrange
            using var context = CreateContext();

            context.Bookings.Add(new Booking
            {
                UserId = "user-1",
                SeatId = 10,
                SessionId = 20,
                Status = status,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var repository = new BookingRepository(context);

            // Act
            var result = await repository.IsSeatBookedForSession(
                requestedSeatId,
                requestedSessionId,
                CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddBooking_SavesBooking()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BookingRepository(context);

            var booking = new Booking
            {
                UserId = "user-1",
                SessionId = 1,
                SeatId = 5,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            await repository.AddBooking(
                booking, CancellationToken.None);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Assert
            var savedBooking = await context.Bookings.SingleAsync();

            Assert.Equal("user-1", savedBooking.UserId);
            Assert.Equal(1, savedBooking.SessionId);
            Assert.Equal(5, savedBooking.SeatId);
            Assert.True(savedBooking.Status);
        }

        [Fact]
        public async Task UpdateBooking_SavesChangedStatus()
        {
            // Arrange
            using var context = CreateContext();

            var booking = new Booking
            {
                UserId = "user-1",
                SessionId = 1,
                SeatId = 1,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            // Detach so the repository must attach the changed booking.
            context.ChangeTracker.Clear();

            var repository = new BookingRepository(context);
            booking.Status = false;

            // Act
            repository.UpdateBooking(booking);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            // Assert
            var updatedBooking = await context.Bookings.FindAsync(
                booking.Id);

            Assert.NotNull(updatedBooking);
            Assert.False(updatedBooking.Status);
        }

        [Fact]
        public async Task DeleteBooking_RemovesBooking()
        {
            // Arrange
            using var context = CreateContext();

            var booking = new Booking
            {
                UserId = "user-1",
                SessionId = 1,
                SeatId = 1,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var repository = new BookingRepository(context);

            // Act
            repository.DeleteBooking(booking);
            await context.SaveChangesAsync();

            // Assert
            Assert.Empty(await context.Bookings.ToListAsync());
        }
    }
}
