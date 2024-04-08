namespace Chinook.Models
{
    public partial class PlaylistTrack
    {
        public long PlaylistId { get; set; }       
        public long TrackId { get; set; }
        public Playlist Playlist { get; set; }
        public Track Track { get; set; }
    }
}
