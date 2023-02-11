using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedListOfAnswerVotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Answers_AnswerId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Vote_AnswerId",
                table: "Votes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vote_AnswerId",
                table: "Votes",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Answers_AnswerId",
                table: "Votes",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
