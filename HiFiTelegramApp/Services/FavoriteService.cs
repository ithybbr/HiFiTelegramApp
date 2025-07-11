using Newtonsoft.Json.Linq;

namespace HiFiTelegramApp.Services;

public class FavoriteService
{
    private readonly IWebHostEnvironment _env;
    
    public FavoriteService(IWebHostEnvironment env)
    {
        this._env = env;
    }
     public Task AddToFavoriteMarkArtists(string artist)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            obj[artist]!["~Favorite"] = true;
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
}