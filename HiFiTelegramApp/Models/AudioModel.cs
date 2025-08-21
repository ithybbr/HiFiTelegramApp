namespace HiFiTelegramApp.Models;

public class AudioModel
{
    public int Id { get; init; }
    public int SongId { get; init; }
    public string Path { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public bool IsFavorite { get; set; } = false;
}