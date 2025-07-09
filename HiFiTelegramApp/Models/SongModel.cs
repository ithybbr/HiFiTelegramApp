namespace HiFiTelegramApp.Models;

public class SongModel
{
    public int Id { get; init; }
    public int SongId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Artist { get; init; } =  string.Empty;
}