using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KmcEventApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVisibilityToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Events");
        }
    }
}
