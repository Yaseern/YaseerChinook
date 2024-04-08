namespace Chinook.Helpers
{
    public static class Constants
    {
        public static readonly string DefaultFavoriteName = "My favorite tracks";
        public static string PlaylistDuplicatedMessage(string value) => $"The playlist with the name {value} already exists. Please choose a different name.";
    }
}
