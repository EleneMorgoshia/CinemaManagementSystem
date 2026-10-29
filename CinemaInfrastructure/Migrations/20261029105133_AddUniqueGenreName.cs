using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueGenreName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Genres_GenreName",
                table: "Genres",
                column: "GenreName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Genres_GenreName",
                table: "Genres");
        }
    }
}
