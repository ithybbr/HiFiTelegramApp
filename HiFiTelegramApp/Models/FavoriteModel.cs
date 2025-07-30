namespace HiFiTelegramApp.Models;

public class FavoriteModel
{
    public string Artist { get; init; } = string.Empty;
    public List<FavoriteSongModel> Songs { get; init; } = [];
}