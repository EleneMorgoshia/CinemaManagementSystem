
using CinemaApplication.Profiles;
using CinemaApplication.Services.Implementations;
using CinemaApplication.Services.Interfaces;
using CinemaDomain.Interfaces;
using CinemaInfrastructure;
using CinemaInfrastructure.Data;
using CinemaInfrastructure.Identity;
using CinemaInfrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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
            //builder.Services.AddSwaggerGen();

            //swagger + (using token)

            builder.Services.AddSwaggerGen(ConfigureSwagger);

            void ConfigureSwagger(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CinemaGo API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid JWT token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    
                        new List<string>()
                    }
                });
            }


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
            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<IGenreRepository, GenreRepository>();
            builder.Services.AddScoped<IUsermManagerService, UserManagerService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IGenreService, GenreService>();

            //AUTO MAPPER ADD
            builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);

            //add identity+(migration (update database) )
            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<CinemaDbContext>();

            //+jwt
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services
                .AddAuthentication(ConfigureAuthentication)
                .AddJwtBearer(ConfigureJwtBearer);

            void ConfigureAuthentication(AuthenticationOptions options)
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }

            void ConfigureJwtBearer(JwtBearerOptions options)
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey!))
                };
            }


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

