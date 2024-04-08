using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinook.Migrations
{
    /// <inheritdoc />
    public partial class _20240408 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Track_TrackId",
                table: "PlaylistTrack");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "PlaylistTrack",
                newName: "TracksTrackId");

            migrationBuilder.RenameColumn(
                name: "PlaylistId",
                table: "PlaylistTrack",
                newName: "PlaylistsPlaylistId");

            migrationBuilder.RenameIndex(
                name: "IFK_PlaylistTrackTrackId",
                table: "PlaylistTrack",
                newName: "IX_PlaylistTrack_TracksTrackId");

            migrationBuilder.CreateTable(
                name: "PlaylistTracks",
                columns: table => new
                {
                    PlaylistId = table.Column<long>(type: "INTEGER", nullable: false),
                    TrackId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTracks", x => new { x.TrackId, x.PlaylistId });
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "PlaylistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "TrackId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_PlaylistId",
                table: "PlaylistTracks",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistsPlaylistId",
                table: "PlaylistTrack",
                column: "PlaylistsPlaylistId",
                principalTable: "Playlist",
                principalColumn: "PlaylistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Track_TracksTrackId",
                table: "PlaylistTrack",
                column: "TracksTrackId",
                principalTable: "Track",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistsPlaylistId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Track_TracksTrackId",
                table: "PlaylistTrack");

            migrationBuilder.DropTable(
                name: "PlaylistTracks");

            migrationBuilder.RenameColumn(
                name: "TracksTrackId",
                table: "PlaylistTrack",
                newName: "TrackId");

            migrationBuilder.RenameColumn(
                name: "PlaylistsPlaylistId",
                table: "PlaylistTrack",
                newName: "PlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTrack_TracksTrackId",
                table: "PlaylistTrack",
                newName: "IFK_PlaylistTrackTrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistId",
                table: "PlaylistTrack",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Track_TrackId",
                table: "PlaylistTrack",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "TrackId");
        }
    }
}
