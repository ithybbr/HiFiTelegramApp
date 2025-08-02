using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ElectronNET.API;
using HiFiTelegramApp.Models;
using Python.Runtime;

namespace HiFiTelegramApp.Services;

public class DownloadService
{
    private readonly IWebHostEnvironment _env;
    
    public DownloadService(IWebHostEnvironment env)
    {
        this._env = env;
        var downloadedIdsPath = Path.Combine(this._env.ContentRootPath, "Resources", "downloaded_songs.json");
        var jsonContent = File.ReadAllText(downloadedIdsPath); // Read the file content as a single string
        this.Downloads = JsonSerializer.Deserialize<List<AudioModel>>(jsonContent) ?? []; // Deserialize the string content
        Console.WriteLine($"Loaded downloads from {Downloads[0].Name}");

        this.IdsToSong = this.Downloads.ToDictionary(x => x.SongId, x => x);
    }
    public List<AudioModel> GetDownloads()
    {
        return this.Downloads ?? [];
    }
    public AudioModel GetDownloadById(int songId)
    {
        return this.IdsToSong[songId];
    }

    public Task AddToDownloads(string artist, int songId, string result)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_songs.json");
            var trimmed = result.Replace($"{_env.ContentRootPath}\\wwwroot", "").Replace('\\','/');
            Console.WriteLine($"Adding songId {songId} with path {trimmed} to downloads.");
            Console.WriteLine($"Result: {result}");
            var text = File.ReadAllTextAsync(DownloadedIdsPath).Result;
            var list = JsonSerializer.Deserialize<List<AudioModel>>(text)
                       ?? [];

            AudioModel audioModel = new()
            {
                Id = list.Count + 1,
                SongId = songId,
                Path = trimmed,
                Artist = artist,
                Name = $"Song {songId}"
            };
            // 2) Append
            list.Add(audioModel);
            this.Downloads.Add(audioModel);
            this.IdsToSong[songId] = audioModel;
            // 3) Save
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var newJson = JsonSerializer.Serialize(list, opts);
            File.WriteAllTextAsync(DownloadedIdsPath, newJson);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task DeleteFromDownloads(int songId)
    {
        try
        {
            var downloadsPath = Path.Combine(_env.ContentRootPath, "Downloads");
            Directory.EnumerateFiles(downloadsPath, $"*{songId}*").ToList().ForEach(File.Delete);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    public Task RemoveFromDownloads(int songId)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_id.txt");
            var content = File.ReadAllText(DownloadedIdsPath);
            content = content.Replace(songId.ToString(), string.Empty);
            File.WriteAllText(DownloadedIdsPath, content);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    private List<AudioModel> Downloads { get; set; } = [];
    private Dictionary<int, AudioModel> IdsToSong { get; set; } = [];
}