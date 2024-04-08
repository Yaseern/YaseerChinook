using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chinook.Services.Playlist
{
    public class PlaylistService(ChinookContext dbContext) : IPlaylistService
    {
        private Task<AuthenticationState> AuthenticationState { get; set; }
        private readonly ChinookContext _dbContext = dbContext;
        private readonly string DefaultFavoriteName = "My favorite tracks";
        private async Task<string> GetUserId()
        {
            var user = (await AuthenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        public async Task<ClientModels.Playlist> GetPlaylistById(long playlistId)
        {
            var currentUserId = await GetUserId();
            return await _dbContext.Playlists
            .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == playlistId)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists
                        .Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId 
                                    && up.Playlist.Name == DefaultFavoriteName)).Any()
                }).ToList()
            })
            .FirstOrDefaultAsync();
        }
    }
}
