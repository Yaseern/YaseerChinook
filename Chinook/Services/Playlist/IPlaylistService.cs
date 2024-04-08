﻿using Chinook.ClientModels;

namespace Chinook.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<List<ClientModels.Playlist>> GetPlaylists();
        Task<ClientModels.Playlist> GetPlaylistById(long playlistId);
        Task FavoriteTrack();
        Task UnFavoriteTrack();
        Task<List<PlaylistTrack>> GetTracksByArtistId(long artistId);
    }
}
