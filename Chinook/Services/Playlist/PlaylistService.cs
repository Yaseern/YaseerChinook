using Chinook.Areas;
using Chinook.ClientModels;
using Chinook.Helpers;
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
                Name = p.Name,
                PlaylistId = p.PlaylistId
            }).ToListAsync();
        }

        public async Task<long> SaveNewPlaylist(string name)
        {
            var isExists = await _dbContext.Playlists.AnyAsync(p => p.Name == name);
            if (isExists)
            {
                throw new ArgumentException(Constants.PlaylistDuplicatedMessage(name));
            }

            var lastId = await _dbContext.Playlists.MaxAsync(p => p.PlaylistId);
            var newPlaylist = new Models.Playlist
            {
                PlaylistId = lastId + 1,
                Name = name
            };
            await _dbContext.Playlists.AddAsync(newPlaylist);
            await _dbContext.SaveChangesAsync();
            return newPlaylist.PlaylistId;
        }

        public async Task<ClientModels.Playlist> GetPlaylistById(long playlistId)
        {
            var currentUserId = await GetUserId();
            var favoritePlaylist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == Constants.DefaultFavoriteName);

            return await _dbContext.Playlists
            .Include(a => a.Tracks)
                .ThenInclude(a => a.Album)
                    .ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == playlistId)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new PlaylistTrack()
                {
                    AlbumTitle = t.Album != null ? t.Album.Title : string.Empty,
                    ArtistName = t.Album != null && t.Album.Artist != null ? t.Album.Artist.Name : string.Empty,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Any(p => p.UserPlaylists != null &&
                        p.UserPlaylists.Any(up => up.UserId == currentUserId && up.PlaylistId == favoritePlaylist.PlaylistId))
                }).ToList()
            })
            .FirstOrDefaultAsync();
        }

        public async Task FavoriteTrack(long trackId)
        {
            var favoritePlaylist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == Constants.DefaultFavoriteName);

            if (favoritePlaylist == null)
            {
                var lastId = await GetLatestPaylistId();
                favoritePlaylist = new Models.Playlist
                {
                    PlaylistId = lastId + 1,
                    Name = Constants.DefaultFavoriteName,
                };
                await _dbContext.Playlists.AddAsync(favoritePlaylist);
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.PlaylistTracks.AddAsync(MapPlaylistTrack(favoritePlaylist.PlaylistId, trackId));
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnFavoriteTrack(long trackId)
        {
            var favoritePlaylist = await MyFavoriteTrackId();

            _dbContext.PlaylistTracks.Remove(MapPlaylistTrack(favoritePlaylist.PlaylistId, trackId));
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PlaylistTrack>> GetTracksByArtistId(long artistId)
        {
            var favoritePlaylist = await MyFavoriteTrackId();

            return await _dbContext.Tracks.Where(a => a.Album != null && a.Album.ArtistId == artistId)
             .Include(a => a.Album)
             .Select(t => new PlaylistTrack()
             {
                 AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                 TrackId = t.TrackId,
                 TrackName = t.Name,
                 IsFavorite = t.PlaylistTracks.Any(up => up.PlaylistId == favoritePlaylist.PlaylistId)
             })
             .ToListAsync();
        }

        private static Models.PlaylistTrack MapPlaylistTrack(long playlistId, long trackId) //TODO: Should move to mapper
        {
            return new Models.PlaylistTrack()
            {
                PlaylistId = playlistId,
                TrackId = trackId
            };
        }

        private async Task<long> GetLatestPaylistId()
        {
            return await _dbContext.Playlists.MaxAsync(p => p.PlaylistId);
        }

        private async Task<Models.Playlist> MyFavoriteTrackId()
        {
            var playlist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == Constants.DefaultFavoriteName);
            if (playlist == null)
            {
                throw new ArgumentException($"{Constants.DefaultFavoriteName} playlist not found");
            }
            return playlist;
        }
    }
}
