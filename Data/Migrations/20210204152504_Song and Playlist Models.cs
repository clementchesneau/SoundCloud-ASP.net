using Microsoft.EntityFrameworkCore.Migrations;

namespace ASPCSoundCloud.Data.Migrations
{
    public partial class SongandPlaylistModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SongDescription",
                table: "Playlist");

            migrationBuilder.DropColumn(
                name: "SongName",
                table: "Playlist");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Playlist",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Playlist",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Playlist");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Playlist");

            migrationBuilder.AddColumn<string>(
                name: "SongDescription",
                table: "Playlist",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SongName",
                table: "Playlist",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
