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
    public class SessionRepositoryTest
    {
        [Fact]
        public async Task GetSessionsByHallId_ReturnsOnlySessionsInRequestedHall()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            const int requestedHallId = 1;
            const int otherHallId = 2;
            const int movieId = 10;
            var startsAt = DateTime.UtcNow.AddDays(1);

            await using (var seedContext = new CinemaDbContext(options))
            {
                seedContext.Halls.AddRange(
                    new Hall { Id = requestedHallId, HallName = "Hall 1" },
                    new Hall { Id = otherHallId, HallName = "Hall 2" });

                seedContext.Movies.Add(new Movie
                {
                    Id = movieId,
                    Title = "Interstellar",
                    Description = "A science fiction movie.",
                    DurationMinutes = 169
                });

                seedContext.Sessions.AddRange(
                    new Session
                    {
                        Id = 101,
                        HallId = requestedHallId,
                        MovieId = movieId,
                        StartsAt = startsAt,
                        Status = true
                    },
                    new Session
                    {
                        Id = 102,
                        HallId = requestedHallId,
                        MovieId = movieId,
                        StartsAt = startsAt.AddHours(3),
                        Status = true
                    },
                    new Session
                    {
                        Id = 103,
                        HallId = otherHallId,
                        MovieId = movieId,
                        StartsAt = startsAt,
                        Status = true
                    });

                await seedContext.SaveChangesAsync();
            }

            await using var context = new CinemaDbContext(options);
            var repository = new SessionRepository(context);

            // Act
            var sessions = await repository.GetSessionsByHallId(
                requestedHallId, CancellationToken.None);

            // Assert
            var sessionIds = sessions
                .Select(session => session.Id)
                .OrderBy(id => id)
                .ToArray();

            Assert.Equal(new[] { 101, 102 }, sessionIds);
        }

        [Fact]
        public async Task GetActiveSessions_ReturnsOnlyActiveFutureSessions()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CinemaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // The repository compares StartsAt with DateTime.Now.
            var now = DateTime.Now;

            await using (var seedContext = new CinemaDbContext(options))
            {
                seedContext.Halls.Add(new Hall { Id = 1, HallName = "Hall 1" });

                seedContext.Movies.Add(new Movie
                {
                    Id = 10,
                    Title = "Interstellar",
                    Description = "A science fiction movie.",
                    DurationMinutes = 169
                });

                seedContext.Sessions.AddRange(
                    new Session
                    {
                        Id = 101,
                        HallId = 1,
                        MovieId = 10,
                        StartsAt = now.AddDays(1),
                        Status = true
                    },
                    new Session
                    {
                        Id = 102,
                        HallId = 1,
                        MovieId = 10,
                        StartsAt = now.AddDays(2),
                        Status = false
                    },
                    new Session
                    {
                        Id = 103,
                        HallId = 1,
                        MovieId = 10,
                        StartsAt = now.AddDays(-1),
                        Status = true
                    });

                await seedContext.SaveChangesAsync();
            }

            await using var context = new CinemaDbContext(options);
            var repository = new SessionRepository(context);

            // Act
            var sessions = await repository.GetActiveSessions(CancellationToken.None);

            // Assert
            var session = Assert.Single(sessions);
            Assert.Equal(101, session.Id);
        }
    }
}
