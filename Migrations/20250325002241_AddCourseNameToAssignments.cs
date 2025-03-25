using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTeacherManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseNameToAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseName",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseName",
                table: "Assignments");
        }
    }
}
