using Chinook.Models;

namespace Chinook.Services.Artist
{
    public interface IArtistService
    {
        Task<List<Models.Artist>> GetArtists();
        Task<Models.Artist> GetArtistById(long artistId);
        Task<List<Album>> GetAlbumsForArtist(int artistId);
    }
}
