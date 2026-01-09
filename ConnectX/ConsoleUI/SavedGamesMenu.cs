using Domain;
using DAL;
using BLL;
using MenuSystem;

namespace ConsoleUI;


public static class SavedGamesMenu
{
    public static string ShowLoadMenu(
        IRepository<GameState> repository,
        Func<GameConfiguration, string, string> loadAndStartGame)
    {
        var savedGames = repository.List();
        
        if (savedGames.Count == 0)
        {
            ShowNoGamesMessage();
            return "";
        }
        
        var loadMenu = new Menu("SELECT SAVED GAME", EMenuLevel.First);
        
        for (int i = 0; i < savedGames.Count; i++)
        {
            var game = savedGames[i];
            loadMenu.AddMenuItem($"{i + 1}", game.description, () =>
            {
                var gameState = repository.Load(game.id);
                
                var loadedConfig = new GameConfiguration
                {
                    BoardWidth = gameState.BoardWidth,
                    BoardHeight = gameState.BoardHeight,
                    WinCondition = gameState.WinCond,
                    BoardType = gameState.BoardType
                };
                
                return loadAndStartGame(loadedConfig, game.id);
            });
        }
        
        return loadMenu.Run();
    }
    
    public static string ShowDeleteMenu(IRepository<GameState> repository)
    {
        var savedGames = repository.List();
        
        if (savedGames.Count == 0)
        {
            ShowNoGamesMessage();
            return "";
        }
        
        var deleteMenu = new Menu("SELECT GAME TO DELETE", EMenuLevel.First);
        
        for (int i = 0; i < savedGames.Count; i++)
        {
            var game = savedGames[i];
            deleteMenu.AddMenuItem($"{i + 1}", game.description, () =>
            {
                return DeleteGameWithConfirmation(repository, game);
            });
        }
        
        return deleteMenu.Run();
    }
    
    public static string ShowEditMenu(IRepository<GameState> repository)
    {
        var savedGames = repository.List();
        
        if (savedGames.Count == 0)
        {
            ShowNoGamesMessage();
            return "";
        }
        
        var editMenu = new Menu("SELECT GAME TO EDIT", EMenuLevel.First);
        
        for (int i = 0; i < savedGames.Count; i++)
        {
            var game = savedGames[i];
            editMenu.AddMenuItem($"{i + 1}", game.description, () =>
            {
                return EditGameInfo(repository, game.id);
            });
        }
        
        return editMenu.Run();
    }
    
    // ========== PRIVATE HELPER METHODS ==========
    
    private static void ShowNoGamesMessage()
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("    NO SAVED GAMES FOUND      ");
        Console.WriteLine("==============================");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
    
    private static string DeleteGameWithConfirmation(
        IRepository<GameState> repository, 
        (string id, string description) game)
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("       CONFIRM DELETE         ");
        Console.WriteLine("==============================");
        Console.WriteLine();
        Console.WriteLine($"Game: {game.description}");
        Console.WriteLine($"ID: {game.id}");
        Console.WriteLine();
        Console.Write("Delete this game? (y/n): ");
        
        var confirm = Console.ReadKey();
        Console.WriteLine();
        
        if (confirm.KeyChar == 'y' || confirm.KeyChar == 'Y')
        {
            repository.Delete(game.id);
            Console.WriteLine();
            Console.WriteLine("==============================");
            Console.WriteLine("      GAME DELETED!           ");
            Console.WriteLine("==============================");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("Deletion cancelled.");
        }
        
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        return "m";
    }
    
    private static string EditGameInfo(IRepository<GameState> repository, string gameId)
    {
        var gameState = repository.Load(gameId);
        
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("       EDIT GAME INFO         ");
        Console.WriteLine("==============================");
        Console.WriteLine();
        
        Console.WriteLine($"Current Player 1: {gameState.Player1Name}");
        Console.Write("New Player 1 name (press Enter to keep): ");
        var newP1 = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newP1))
        {
            gameState.Player1Name = newP1;
        }
        
        Console.WriteLine();
        Console.WriteLine($"Current Player 2: {gameState.Player2Name}");
        Console.Write("New Player 2 name (press Enter to keep): ");
        var newP2 = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newP2))
        {
            gameState.Player2Name = newP2;
        }
        
        repository.Save(gameState);
        
        Console.WriteLine();
        Console.WriteLine("==============================");
        Console.WriteLine("      GAME UPDATED!           ");
        Console.WriteLine("==============================");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        
        return "m";
    }
}