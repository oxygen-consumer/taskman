using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskmanAPI.Migrations
{
    /// <inheritdoc />
    public partial class mergefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjTasks_Users_UserId",
                table: "ProjTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjTasks_UserId",
                table: "ProjTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjTasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "ProjTasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProjTasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ProjTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ProjTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ProjTasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProjTaskUser",
                columns: table => new
                {
                    TasksId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjTaskUser", x => new { x.TasksId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ProjTaskUser_ProjTasks_TasksId",
                        column: x => x.TasksId,
                        principalTable: "ProjTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjTaskUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjTasks_ProjectId",
                table: "ProjTasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjTaskUser_UsersId",
                table: "ProjTaskUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjTasks_Projects_ProjectId",
                table: "ProjTasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjTasks_Projects_ProjectId",
                table: "ProjTasks");

            migrationBuilder.DropTable(
                name: "ProjTaskUser");

            migrationBuilder.DropIndex(
                name: "IX_ProjTasks_ProjectId",
                table: "ProjTasks");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "ProjTasks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProjTasks");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ProjTasks");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjTasks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ProjTasks");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProjTasks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjTasks_UserId",
                table: "ProjTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjTasks_Users_UserId",
                table: "ProjTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
