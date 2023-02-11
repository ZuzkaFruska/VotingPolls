using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAnswerForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_VotingPolls_VotingPollId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_VotingPollId",
                table: "Answers");

            migrationBuilder.AlterColumn<int>(
                name: "VotingPollId",
                table: "Answers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VotingPollId",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_VotingPollId",
                table: "Answers",
                column: "VotingPollId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_VotingPolls_VotingPollId",
                table: "Answers",
                column: "VotingPollId",
                principalTable: "VotingPolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
