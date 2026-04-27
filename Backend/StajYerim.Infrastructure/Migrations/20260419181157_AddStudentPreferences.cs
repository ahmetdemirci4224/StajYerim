using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajYerim.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InterestedRoles",
                table: "StudentProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLocation",
                table: "StudentProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredWorkModel",
                table: "StudentProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestedRoles",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "PreferredLocation",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "PreferredWorkModel",
                table: "StudentProfiles");
        }
    }
}
