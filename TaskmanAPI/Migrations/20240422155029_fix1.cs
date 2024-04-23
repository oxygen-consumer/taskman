using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskmanAPI.Migrations
{
    /// <inheritdoc />
    public partial class fix1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjTaskUser_ProjTasks_TasksId",
                table: "ProjTaskUser");

            migrationBuilder.RenameColumn(
                name: "TasksId",
                table: "ProjTaskUser",
                newName: "ProjTasksId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjTaskUser_ProjTasks_ProjTasksId",
                table: "ProjTaskUser",
                column: "ProjTasksId",
                principalTable: "ProjTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjTaskUser_ProjTasks_ProjTasksId",
                table: "ProjTaskUser");

            migrationBuilder.RenameColumn(
                name: "ProjTasksId",
                table: "ProjTaskUser",
                newName: "TasksId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjTaskUser_ProjTasks_TasksId",
                table: "ProjTaskUser",
                column: "TasksId",
                principalTable: "ProjTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
