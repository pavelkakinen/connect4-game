using System.Text.Json;
using BLL;

namespace DAL;

/// <summary>
/// Конвертер между GameBrain (BLL) и SavedGame (DAL)
/// </summary>
public static class GameMapper
{
    /// <summary>
    /// Преобразовать GameBrain в SavedGame для сохранения
    /// </summary>
    public static SavedGame ToSavedGame(GameBrain gameBrain, string gameName)
    {
        var config = gameBrain.GetConfiguration();
        var board = gameBrain.GetBoard();
        
        // Сериализуем доску (2D массив) в JSON строку
        var boardState = JsonSerializer.Serialize(board);
        
        return new SavedGame
        {
            GameName = gameName,
            SavedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            
            // Configuration
            ConfigName = config.Name,
            BoardWidth = config.BoardWidth,
            BoardHeight = config.BoardHeight,
            WinCondition = config.WinCondition,
            BoardType = config.BoardType.ToString(),
            P1Type = config.P1Type.ToString(),
            P2Type = config.P2Type.ToString(),
            
            // Game State
            Player1Name = gameBrain.Player1Name,
            Player2Name = gameBrain.Player2Name,
            NextMoveByX = gameBrain.NextMoveByRed,
            GameOver = gameBrain.GameOver,
            Winner = gameBrain.Winner.ToString(),
            MoveCount = gameBrain.MoveCount,
            BoardState = boardState
        };
    }
    
    /// <summary>
    /// Восстановить GameBrain из SavedGame
    /// </summary>
    public static GameBrain ToGameBrain(SavedGame savedGame)
    {
        // Восстанавливаем конфигурацию
        var config = new GameConfiguration
        {
            Name = savedGame.ConfigName,
            BoardWidth = savedGame.BoardWidth,
            BoardHeight = savedGame.BoardHeight,
            WinCondition = savedGame.WinCondition,
        };
        
        // Парсим enum'ы из строк
        config.SetBoardType(Enum.Parse<EBoardType>(savedGame.BoardType));
        config.SetP1Type(Enum.Parse<EPlayerType>(savedGame.P1Type));
        config.SetP2Type(Enum.Parse<EPlayerType>(savedGame.P2Type));
        
        // Создаем GameBrain с восстановленной конфигурацией
        var gameBrain = new GameBrain(config, savedGame.Player1Name, savedGame.Player2Name);
        
        // Восстанавливаем состояние игры
        gameBrain.NextMoveByRed = savedGame.NextMoveByX;
        gameBrain.GameOver = savedGame.GameOver;
        gameBrain.Winner = Enum.Parse<ECellState>(savedGame.Winner);
        gameBrain.MoveCount = savedGame.MoveCount;
        
        // Десериализуем доску из JSON строки
        var board = JsonSerializer.Deserialize<ECellState[,]>(savedGame.BoardState);
        if (board != null)
        {
            gameBrain.SetBoard(board);
        }
        
        return gameBrain;
    }
}