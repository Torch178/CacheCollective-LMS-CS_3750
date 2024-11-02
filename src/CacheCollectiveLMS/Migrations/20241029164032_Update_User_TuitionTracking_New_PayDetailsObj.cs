using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesMovie.Migrations
{
    /// <inheritdoc />
    public partial class Update_User_TuitionTracking_New_PayDetailsObj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TuitionId",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "refundAmt",
                table: "User",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tuitionDue",
                table: "User",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tuitionPaid",
                table: "User",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<long>(type: "bigint", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    semester = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_Id_userId",
                table: "PaymentDetails",
                columns: new[] { "Id", "userId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "TuitionId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "refundAmt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "tuitionDue",
                table: "User");

            migrationBuilder.DropColumn(
                name: "tuitionPaid",
                table: "User");
        }
    }
}
