using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskmanAPI.Migrations
{
    /// <inheritdoc />
    public partial class removeprojectownercolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectOwner",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectOwner",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
