namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<ClientModels.Playlist> GetPlaylistById(long playlistId);
    }
}
