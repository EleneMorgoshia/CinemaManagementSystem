
using CinemaApplication.Profiles;
using CinemaApplication.Services.Implementations;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using CinemaInfrastructure;
using CinemaInfrastructure.Data;
using CinemaInfrastructure.Identity;
using CinemaInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //My services, context and repositories, unit of work
            builder.Services.AddDbContext<CinemaDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration
                    .GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IUserRepository, UserManagerRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IHallRepository, HallRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<ISeatRepository,  SeatRepository>();
            builder.Services.AddScoped<IGenreRepository, GenreRepository>();
            builder.Services.AddScoped<IUsermManagerService, UserManagerService>();
            builder.Services.AddScoped<IUserService, UserService>();

            //AUTO MAPPER ADD
            builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);

            //add identity+(migration (update database) )
            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<CinemaDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                //app.UseSwaggerUI();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CinemaGo API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

