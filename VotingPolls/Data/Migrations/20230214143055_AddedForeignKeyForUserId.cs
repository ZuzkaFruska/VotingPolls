using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedForeignKeyForUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotingPolls_AspNetUsers_OwnerId",
                table: "VotingPolls");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "VotingPolls",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_VotingPolls_OwnerId",
                table: "VotingPolls",
                newName: "IX_VotingPolls_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Votes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId",
                table: "Votes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_AspNetUsers_UserId",
                table: "Votes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_VotingPolls_AspNetUsers_UserId",
                table: "VotingPolls",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_AspNetUsers_UserId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_VotingPolls_AspNetUsers_UserId",
                table: "VotingPolls");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "VotingPolls",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_VotingPolls_UserId",
                table: "VotingPolls",
                newName: "IX_VotingPolls_OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Votes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_VotingPolls_AspNetUsers_OwnerId",
                table: "VotingPolls",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
