using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskmanAPI.Migrations
{
    /// <inheritdoc />
    public partial class addusertaskrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjTaskUser_AspNetUsers_UsersId",
                table: "ProjTaskUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjTaskUser_ProjTasks_TasksId",
                table: "ProjTaskUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjTaskUser",
                table: "ProjTaskUser");

            migrationBuilder.RenameTable(
                name: "ProjTaskUser",
                newName: "UserTasks");

            migrationBuilder.RenameIndex(
                name: "IX_ProjTaskUser_UsersId",
                table: "UserTasks",
                newName: "IX_UserTasks_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTasks",
                table: "UserTasks",
                columns: new[] { "TasksId", "UsersId" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameTable(
                name: "UserTasks",
                newName: "ProjTaskUser");

            migrationBuilder.RenameIndex(
                name: "IX_UserTasks_UsersId",
                table: "ProjTaskUser",
                newName: "IX_ProjTaskUser_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjTaskUser",
                table: "ProjTaskUser",
                columns: new[] { "TasksId", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjTaskUser_AspNetUsers_UsersId",
                table: "ProjTaskUser",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
