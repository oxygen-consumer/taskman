using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskmanAPI.Migrations
{
    /// <inheritdoc />
    public partial class projcontrollermig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePerProjects_Projects_ProjectId",
                table: "RolePerProjects");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePerProjects_Projects_ProjectId",
                table: "RolePerProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePerProjects_Projects_ProjectId",
                table: "RolePerProjects");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePerProjects_Projects_ProjectId",
                table: "RolePerProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
