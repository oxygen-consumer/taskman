using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskmanAPI.Migrations
{
    /// <inheritdoc />
    public partial class implementUserTasksexplicitly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_AspNetUsers_UsersId",
                table: "UserTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_ProjTasks_TasksId",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTasks",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_UsersId",
                table: "UserTasks");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "UserTasks",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "TasksId",
                table: "UserTasks",
                newName: "TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTasks",
                table: "UserTasks",
                columns: new[] { "UserId", "TaskId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TaskId",
                table: "UserTasks",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_AspNetUsers_UserId",
                table: "UserTasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_ProjTasks_TaskId",
                table: "UserTasks",
                column: "TaskId",
                principalTable: "ProjTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_AspNetUsers_UserId",
                table: "UserTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_ProjTasks_TaskId",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTasks",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_TaskId",
                table: "UserTasks");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "UserTasks",
                newName: "TasksId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserTasks",
                newName: "UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTasks",
                table: "UserTasks",
                columns: new[] { "TasksId", "UsersId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_UsersId",
                table: "UserTasks",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_AspNetUsers_UsersId",
                table: "UserTasks",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_ProjTasks_TasksId",
                table: "UserTasks",
                column: "TasksId",
                principalTable: "ProjTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
