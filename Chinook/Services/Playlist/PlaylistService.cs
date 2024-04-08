using Chinook.Areas;
using Chinook.ClientModels;
using Chinook.Helpers;
using Chinook.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chinook.Services.Playlist
{
    public class PlaylistService(ChinookContext dbContext, ICustomAuthenticationService authenticationService) : IPlaylistService
    {
        private readonly ChinookContext _dbContext = dbContext;
        
        private async Task<string> GetUserId()
        {
            var user = (await authenticationService.GetAuthenticationStateAsync()).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value ?? string.Empty;
            return userId;
        }

        public async Task<List<ClientModels.Playlist>> GetPlaylists()
        {
            return await _dbContext.Playlists.Select(p => new ClientModels.Playlist()
            {
                Name = p.Name
            }).ToListAsync();
        }

        public async Task<ClientModels.Playlist> GetPlaylistById(long playlistId)
        {
            var currentUserId = await GetUserId();
            return await _dbContext.Playlists
            .Include(a => a.Tracks)
                .ThenInclude(a => a.Album)
                    .ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == playlistId)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = t.Album != null ? t.Album.Title : string.Empty,
                    ArtistName = t.Album == null || t.Album.Artist == null ? string.Empty : t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Any(p => p.UserPlaylists != null && 
                        p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == Constants.DefaultFavoriteName))
                }).ToList()
            })
            .FirstOrDefaultAsync();
        }

        public async Task FavoriteTrack()
        {
            var currentUserId = await GetUserId();
            var favoritePlaylist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == Constants.DefaultFavoriteName);

            if(favoritePlaylist == null) {
                favoritePlaylist = new Models.Playlist
                {
                    PlaylistId = 100,
                    Name = Constants.DefaultFavoriteName,
                };
                await _dbContext.Playlists.AddAsync(favoritePlaylist);
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.UserPlaylists.AddAsync(new Models.UserPlaylist()
            {
                PlaylistId = favoritePlaylist.PlaylistId,
                UserId = currentUserId
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnFavoriteTrack()
        {
            var currentUserId = await GetUserId();
            var favoritePlaylist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == Constants.DefaultFavoriteName);

            _dbContext.UserPlaylists.Remove(new Models.UserPlaylist()
            {
                PlaylistId = favoritePlaylist.PlaylistId,
                UserId = currentUserId
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PlaylistTrack>> GetTracksByArtistId(long artistId)
        {
            var currentUserId = await GetUserId();
            return await _dbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
             .Include(a => a.Album)
             .Select(t => new PlaylistTrack()
             {
                 AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                 TrackId = t.TrackId,
                 TrackName = t.Name,
                 IsFavorite = t.Playlists.Where(p => 
                    p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == Constants.DefaultFavoriteName)).Any()
             })
             .ToListAsync();
        }
    }
}
