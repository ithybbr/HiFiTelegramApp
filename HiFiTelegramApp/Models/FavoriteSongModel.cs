namespace HiFiTelegramApp.Models;

public class FavoriteSongModel
{
    public int SongId { get; init; }
    public string Artist { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}