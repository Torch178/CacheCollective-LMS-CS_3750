using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesMovie.Migrations
{
    /// <inheritdoc />
    public partial class Mod_DataContext_PayDetails_RemoveContraint_UniqueUserIdId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentDetails_Id_userId",
                table: "PaymentDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_Id_userId",
                table: "PaymentDetails",
                columns: new[] { "Id", "userId" },
                unique: true);
        }
    }
}
