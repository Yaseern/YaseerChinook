using Chinook.ClientModels;

namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<List<ClientModels.Playlist>> GetPlaylists();
        Task<ClientModels.Playlist> GetPlaylistById(long playlistId);
        Task<long> SaveNewPlaylist(string name);
        Task FavoriteTrack(long trackId);
        Task UnFavoriteTrack(long trackId);
        Task<List<PlaylistTrack>> GetTracksByArtistId(long artistId);
    }
}
