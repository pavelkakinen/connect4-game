using System.Text.Json;
using BLL;

namespace DAL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    public List<string> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var result = new List<string>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json")) continue;
            
            result.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        return result;
    }

    // TODO: what if we have to update already existing config (with file name change)
    public string Save(GameConfiguration data)
    {
        var jsonStr = JsonSerializer.Serialize(data);
        
        //TODO: sanitize data.Name, its unsafe to use it directly 
        var fileName = $"{data.Name} - {data.BoardWidth}x{data.BoardHeight} - win:{data.WinCondition}" + ".json";
        var fullFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + fileName;
        
        File.WriteAllText(fullFileName, jsonStr);
        
        return fileName;
    }

    public GameConfiguration Load(string id)
    {
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + id + ".json";
        var jsonText = File.ReadAllText(jsonFileName);
        var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);

        return conf ?? throw new Exception("Json deserialization returned null. Data:  " + jsonText);
    }

    public void Delete(string id)
    {
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + id + ".json";
        if (File.Exists(jsonFileName))
        {
            File.Delete(jsonFileName);
        }
    }
}