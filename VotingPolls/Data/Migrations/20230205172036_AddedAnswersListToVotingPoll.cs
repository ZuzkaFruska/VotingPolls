using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedAnswersListToVotingPoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Answers_VotingPollId",
                table: "Answers",
                column: "VotingPollId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_VotingPolls_VotingPollId",
                table: "Answers",
                column: "VotingPollId",
                principalTable: "VotingPolls",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_VotingPolls_VotingPollId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_VotingPollId",
                table: "Answers");
        }
    }
}
