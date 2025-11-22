namespace ConsoleUI;

public static class SavedGamesUI
{
    /// <summary>
    /// Показать список всех сохраненных игр
    /// </summary>
    public static void ShowGamesList(List<string> games)
    {
        Console.Clear();
        Console.WriteLine("=== Saved Games ===\n");
        
        if (games.Count == 0)
        {
            Console.WriteLine("No saved games found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }
        
        Console.WriteLine("Available saved games:");
        for (int i = 0; i < games.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {games[i]}");
        }
        Console.WriteLine();
    }
    
    /// <summary>
    /// Спросить у пользователя имя для сохранения игры
    /// </summary>
    public static string? GetGameNameInput()
    {
        Console.Write("Enter game name to save (or 'b' to cancel): ");
        var input = Console.ReadLine()?.Trim();
        
        if (string.IsNullOrEmpty(input) || input.ToLower() == "b")
        {
            return null;
        }
        
        return input;
    }
    
    /// <summary>
    /// Спросить у пользователя какую игру загрузить
    /// </summary>
    public static string? SelectGameToLoad(List<string> games)
    {
        if (games.Count == 0)
        {
            return null;
        }
        
        while (true)
        {
            Console.Write($"Select game number (1-{games.Count}) or 'b' to cancel: ");
            var input = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(input) || input.ToLower() == "b")
            {
                return null;
            }
            
            if (int.TryParse(input, out var index))
            {
                if (index >= 1 && index <= games.Count)
                {
                    return games[index - 1];
                }
            }
            
            Console.WriteLine($"Please enter a number between 1 and {games.Count}.");
        }
    }
    
    /// <summary>
    /// Спросить подтверждение у пользователя
    /// </summary>
    public static bool ConfirmAction(string message)
    {
        Console.Write($"{message} (y/n): ");
        var input = Console.ReadLine()?.Trim().ToLower();
        return input == "y" || input == "yes";
    }
    
    /// <summary>
    /// Показать сообщение об успехе
    /// </summary>
    public static void ShowSuccess(string message)
    {
        Console.WriteLine($"\n✓ {message}");
        Thread.Sleep(1500);
    }
    
    /// <summary>
    /// Показать сообщение об ошибке
    /// </summary>
    public static void ShowError(string message)
    {
        Console.WriteLine($"\n✗ {message}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}