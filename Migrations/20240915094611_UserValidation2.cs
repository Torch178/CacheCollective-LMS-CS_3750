using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesMovie.Migrations
{
    /// <inheritdoc />
    public partial class UserValidation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInstructor",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInstructor",
                table: "User");
        }
    }
}
