using System.Text.Json;
using DAL.Helpers;

namespace DAL.Json;

public class GameStateRepositoryJson : IRepository<GameState>
{
    public List<(string id, string description)> List()
    {
        var dir = FilesystemHelpers.GetGamesDirectory();
        var res = new List<(string id, string description)>();
        
        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json")) continue;
            
            try
            {
                var jsonText = File.ReadAllText(fullFileName);
                var game = JsonSerializer.Deserialize<GameState>(jsonText);
                
                if (game != null)
                {
                    res.Add((
                        game.GameId,
                        $"{game.Player1Name} vs {game.Player2Name} - {game.SavedAt:yyyy-MM-dd HH:mm}"
                    ));
                }
            }
            catch
            {
                continue;
            }
        }
        
        return res;
    }
    
    public async Task<List<(string id, string description)>> ListAsync()
    {
        return List();
    }
    
    public string Save(GameState data)
    {
        if (string.IsNullOrEmpty(data.GameId))
        {
            data.GameId = GenerateGameId();
        }
        
        Console.WriteLine($" JSON SAVE: {data.GameId} at {data.SavedAt:HH:mm:ss.fff}");
        
        
        var jsonStr = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        var fileName = $"{data.GameId}.json";
        var fullFileName = FilesystemHelpers.GetGamesDirectory() + Path.DirectorySeparatorChar + fileName;
        
        File.WriteAllText(fullFileName, jsonStr);
        return data.GameId;
    }
    
    public GameState Load(string id)
    {
        var jsonFileName = FilesystemHelpers.GetGamesDirectory() + Path.DirectorySeparatorChar + id;
        
        if (!File.Exists(jsonFileName))
        {
            jsonFileName += ".json";
        }
        
        var jsonText = File.ReadAllText(jsonFileName);
        var game = JsonSerializer.Deserialize<GameState>(jsonText);
        
        return game ?? throw new NullReferenceException("Json deserialization returned null. Data: " + jsonText);
    }
    
    public void Delete(string id)
    {
        var jsonFileName = FilesystemHelpers.GetGamesDirectory() + Path.DirectorySeparatorChar + id;
        
        if (!jsonFileName.EndsWith(".json"))
        {
            jsonFileName += ".json";
        }
        
        if (File.Exists(jsonFileName))
        {
            File.Delete(jsonFileName);
        }
    }
    
    private static string GenerateGameId()
    {
        return $"game_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString()[..8]}";
    }
}