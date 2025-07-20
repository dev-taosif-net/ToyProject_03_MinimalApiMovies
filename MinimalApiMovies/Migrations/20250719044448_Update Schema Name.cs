using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalApiMovies.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Genre");

            migrationBuilder.RenameTable(
                name: "Genres",
                newName: "Genres",
                newSchema: "Genre");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Genre",
                table: "Genres",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Genres",
                schema: "Genre",
                newName: "Genres");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Genres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
