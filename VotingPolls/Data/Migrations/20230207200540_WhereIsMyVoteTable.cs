using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class WhereIsMyVoteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_VotingPolls_VotingPollId",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vote",
                table: "Votes");

            migrationBuilder.RenameTable(
                name: "Votes",
                newName: "Votes");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_VotingPollId",
                table: "Votes",
                newName: "IX_Votes_VotingPollId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_VotingPolls_VotingPollId",
                table: "Votes",
                column: "VotingPollId",
                principalTable: "VotingPolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_VotingPolls_VotingPollId",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.RenameTable(
                name: "Votes",
                newName: "Vote");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_VotingPollId",
                table: "Vote",
                newName: "IX_Vote_VotingPollId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vote",
                table: "Vote",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_VotingPolls_VotingPollId",
                table: "Vote",
                column: "VotingPollId",
                principalTable: "VotingPolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
