using Microsoft.EntityFrameworkCore.Migrations;

namespace STOApi.Migrations
{
    public partial class ChangedTournament : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Tournaments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Tournaments",
                nullable: true);
        }
    }
}
