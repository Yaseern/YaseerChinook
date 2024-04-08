using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Artist
{
    public class ArtistService(ChinookContext dbContext) : IArtistService
    {
        private readonly ChinookContext _dbContext = dbContext;

        public async Task<List<Models.Artist>> GetArtists()
        {
            return await _dbContext.Artists
                .Include(a => a.Albums)
                .ToListAsync();
        }

        public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        {
            return await _dbContext.Albums
                .Where(a => a.ArtistId == artistId)
                .ToListAsync();
        }
    }
}
