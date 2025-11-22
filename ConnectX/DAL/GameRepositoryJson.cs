using System.Text.Json;

namespace DAL;

public class GameRepositoryJson : IRepository<SavedGame>
{
    /// <summary>
    /// Получить список всех сохраненных игр
    /// Возвращает список имен файлов (без .json)
    /// </summary>
    public List<string> List()
    {
        var dir = FilesystemHelpers.GetGameDirectory();
        var result = new List<string>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json")) continue;
            
            result.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        return result;
    }

    /// <summary>
    /// Сохранить игру в JSON файл
    /// Возвращает имя файла
    /// </summary>
    public string Save(SavedGame data)
    {
        // Обновляем время сохранения
        data.SavedAt = data.SavedAt == default ? DateTime.Now : data.SavedAt;
        data.UpdatedAt = DateTime.Now;
        
        // Сериализуем в JSON с красивым форматированием
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonStr = JsonSerializer.Serialize(data, options);
        
        // Создаем безопасное имя файла
        // TODO: sanitize data.GameName, its unsafe to use it directly
        var fileName = $"{data.GameName} - {data.SavedAt:yyyy-MM-dd_HH-mm-ss}.json";
        var fullFileName = FilesystemHelpers.GetGameDirectory() + Path.DirectorySeparatorChar + fileName;
        
        // Сохраняем в файл
        File.WriteAllText(fullFileName, jsonStr);
        
        return fileName;
    }

    /// <summary>
    /// Загрузить игру из JSON файла
    /// </summary>
    public SavedGame Load(string id)
    {
        var jsonFileName = FilesystemHelpers.GetGameDirectory() + Path.DirectorySeparatorChar + id + ".json";
        
        if (!File.Exists(jsonFileName))
        {
            throw new FileNotFoundException($"Game file not found: {id}");
        }
        
        var jsonText = File.ReadAllText(jsonFileName);
        var game = JsonSerializer.Deserialize<SavedGame>(jsonText);

        return game ?? throw new Exception("Json deserialization returned null. Data: " + jsonText);
    }

    /// <summary>
    /// Удалить сохраненную игру
    /// </summary>
    public void Delete(string id)
    {
        var jsonFileName = FilesystemHelpers.GetGameDirectory() + Path.DirectorySeparatorChar + id + ".json";
        
        if (File.Exists(jsonFileName))
        {
            File.Delete(jsonFileName);
        }
    }
}