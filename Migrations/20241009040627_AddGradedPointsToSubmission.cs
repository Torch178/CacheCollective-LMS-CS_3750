using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesMovie.Migrations
{
    /// <inheritdoc />
    public partial class AddGradedPointsToSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GradedPoints",
                table: "Submission",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_AssignmentId",
                table: "Submission",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Assignment_AssignmentId",
                table: "Submission",
                column: "AssignmentId",
                principalTable: "Assignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Assignment_AssignmentId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_AssignmentId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "GradedPoints",
                table: "Submission");
        }
    }
}
