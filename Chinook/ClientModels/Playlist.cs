namespace Chinook.ClientModels;

public class Playlist
{
    public string Name { get; set; }
    public long PlaylistId { get; set; }
    public List<PlaylistTrack> Tracks { get; set; }
}