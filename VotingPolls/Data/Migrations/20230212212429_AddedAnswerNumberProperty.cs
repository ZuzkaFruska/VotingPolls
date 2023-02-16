using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPolls.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedAnswerNumberProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Answers");
        }
    }
}
