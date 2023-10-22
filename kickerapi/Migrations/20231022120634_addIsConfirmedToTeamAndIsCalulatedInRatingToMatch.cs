using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kickerapi.Migrations
{
    /// <inheritdoc />
    public partial class addIsConfirmedToTeamAndIsCalulatedInRatingToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Teams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCalculatedInRating",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsCalculatedInRating",
                table: "Matches");
        }
    }
}
