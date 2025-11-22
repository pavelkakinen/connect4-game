using BLL;
using ConsoleUI;
using DAL;

namespace ConsoleApp;

/// <summary>
/// Контроллер для управления сохраненными играми
/// Связывает UI, Repository и GameBrain
/// </summary>
public class SavedGamesController
{
    private readonly IRepository<SavedGame> _repository;

    public SavedGamesController(IRepository<SavedGame> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Показать список всех сохраненных игр
    /// </summary>
    public string ListGames()
    {
        var games = _repository.List();
        SavedGamesUI.ShowGamesList(games);
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        return "b";
    }

    /// <summary>
    /// Сохранить текущую игру
    /// </summary>
    public string SaveCurrentGame(GameBrain gameBrain)
    {
        Console.Clear();
        Console.WriteLine("=== Save Game ===\n");
        
        var gameName = SavedGamesUI.GetGameNameInput();
        if (gameName == null)
        {
            return "b"; // Пользователь отменил
        }
        
        try
        {
            // Конвертируем GameBrain в SavedGame
            var savedGame = GameMapper.ToSavedGame(gameBrain, gameName);
            
            // Сохраняем через репозиторий
            var fileName = _repository.Save(savedGame);
            
            SavedGamesUI.ShowSuccess($"Game saved as: {fileName}");
        }
        catch (Exception ex)
        {
            SavedGamesUI.ShowError($"Failed to save game: {ex.Message}");
        }
        
        return "b";
    }

    /// <summary>
    /// Загрузить сохраненную игру
    /// </summary>
    public GameBrain? LoadGame()
    {
        var games = _repository.List();
        SavedGamesUI.ShowGamesList(games);
        
        if (games.Count == 0)
        {
            return null;
        }
        
        var selectedGame = SavedGamesUI.SelectGameToLoad(games);
        if (selectedGame == null)
        {
            return null; // Пользователь отменил
        }
        
        try
        {
            // Загружаем SavedGame из файла
            var savedGame = _repository.Load(selectedGame);
            
            // Конвертируем обратно в GameBrain
            var gameBrain = GameMapper.ToGameBrain(savedGame);
            
            SavedGamesUI.ShowSuccess($"Game '{savedGame.GameName}' loaded!");
            return gameBrain;
        }
        catch (Exception ex)
        {
            SavedGamesUI.ShowError($"Failed to load game: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Удалить сохраненную игру
    /// </summary>
    public string DeleteGame()
    {
        var games = _repository.List();
        SavedGamesUI.ShowGamesList(games);
        
        if (games.Count == 0)
        {
            return "b";
        }
        
        var selectedGame = SavedGamesUI.SelectGameToLoad(games);
        if (selectedGame == null)
        {
            return "b"; // Пользователь отменил
        }
        
        if (SavedGamesUI.ConfirmAction($"Delete game '{selectedGame}'?"))
        {
            try
            {
                _repository.Delete(selectedGame);
                SavedGamesUI.ShowSuccess($"Game '{selectedGame}' deleted!");
            }
            catch (Exception ex)
            {
                SavedGamesUI.ShowError($"Failed to delete game: {ex.Message}");
            }
        }
        
        return "b";
    }
}