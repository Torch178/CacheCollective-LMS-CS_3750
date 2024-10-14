using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesMovie.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorCommentsToSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Assignment_AssignmentId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_AssignmentId",
                table: "Submission");

            migrationBuilder.AddColumn<string>(
                name: "InstructorComments",
                table: "Submission",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstructorComments",
                table: "Submission");

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
    }
}
