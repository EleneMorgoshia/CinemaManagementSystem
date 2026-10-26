using CinemaDomain.Models.Entities;
using CinemaInfrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaInfrastructure.Data
{
    public class CinemaDbContext : IdentityDbContext<ApplicationUser>
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) 
        { 
        
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Movie vs MovieGenre
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.MovieGenreRecords)
                .WithOne(mg => mg.Movie)
                .HasForeignKey(mg => mg.MovieId);

            //Movie vs Session
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.MovieSessions)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);

            //Hall vs Seat
            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Seats)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId);

            //Hall vs Session  
            modelBuilder.Entity<Hall>()
                .HasMany(h => h.MovieSessions)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId);

            //MovieGenre vs Genre
            modelBuilder.Entity<Genre>()
                .HasMany(g => g.MovieGenreRecords)
                .WithOne(mg => mg.Genre)
                .HasForeignKey(mg => mg.GenreId);

            //User vs Booking
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            //Session vs Booking
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Session)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.NoAction);
            //Seat vs Booking
            modelBuilder.Entity<Seat>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Seat)
                .HasForeignKey(b => b.SeatId)
                .OnDelete(DeleteBehavior.Cascade);

            //Ticket vs Booking
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithOne(b => b.Ticket)
                .HasForeignKey<Ticket>(t => t.BookingId);

            //must need!
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            //requirments
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DurationMinutes).IsRequired();
                entity.Property(e => e.AgeRating).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(2000);
            });

           
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.Property(e => e.GenreName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Hall>(entity =>
            {
                entity.Property(e => e.HallName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.Property(e => e.RowNumber).IsRequired();
                entity.Property(e => e.SeatNumber).IsRequired();
                entity.Property(e => e.IsVip).IsRequired();
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.StartsAt).IsRequired();
                entity.Property(e => e.StandardPrice).HasPrecision(18, 2);
                entity.Property(e => e.VipPrice).HasPrecision(18, 2);
                entity.Property(e => e.Status).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(e => e.TicketCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FinalPrice).HasPrecision(18, 2);
            });
        }
    }
}
