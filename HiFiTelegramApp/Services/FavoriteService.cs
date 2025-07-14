using Newtonsoft.Json.Linq;

namespace HiFiTelegramApp.Services;

public class FavoriteService
{
    private readonly IWebHostEnvironment _env;
    
    public FavoriteService(IWebHostEnvironment env)
    {
        this._env = env;
        var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.txt");
        this.Favorites = [.. File.ReadAllLines(favoritesPath)];
    }
    public List<string> GetFavoriteArtists()
    {
        try
        {
            return this.Favorites;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading favorites: {ex.Message}");
            return [];
        }
    }
     public Task AddToFavoriteMarkArtists(string artist)
    {
        Console.WriteLine($"Adding {artist} to favorites");
        try
        {
            var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.txt");
            if(this.Favorites.Contains(artist))
            {
                Console.WriteLine($"Artist {artist} is already in favorites");
                return Task.CompletedTask;
            }
            File.AppendAllLines(favoritesPath, [artist]);
            this.Favorites.Add(artist);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task RemoveFromFavoriteMarkArtists(string artist)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            obj[artist]!["~Favorite"] = false;
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    
    public Task AddToFavoriteSongs(string artist, string song, int songId)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            _ = obj[artist]!["Favorites"]?.Append(songId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    
    public Task RemoveFromFavoriteSongs(string artist, string song, int songId)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            var favs = obj[artist]?["Favorites"];
            for (var i = 0; i < favs?.Count(); i++)
            {
                if (favs[i]?.Value<int>() == songId)
                {
                    favs[i]?.Remove();
                }
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    private List<string> Favorites { get; set; } = [];
}