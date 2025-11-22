namespace DAL;

/// <summary>
/// Модель для сохранения состояния игры
/// Содержит всю информацию чтобы восстановить игру
/// </summary>
public class SavedGame
{
    // Metadata (информация о сохранении)
    public string GameName { get; set; } = default!;
    public DateTime SavedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Game Configuration (настройки игры)
    public string ConfigName { get; set; } = default!;
    public int BoardWidth { get; set; }
    public int BoardHeight { get; set; }
    public int WinCondition { get; set; }
    public string BoardType { get; set; } = default!; // "Rectangle" or "Cylinder"
    
    // Player Types
    public string P1Type { get; set; } = default!; // "Human" or "Computer"
    public string P2Type { get; set; } = default!;
    
    // Game State (текущее состояние)
    public string Player1Name { get; set; } = default!;
    public string Player2Name { get; set; } = default!;
    public bool NextMoveByX { get; set; }
    public bool GameOver { get; set; }
    public string Winner { get; set; } = default!; // "Empty", "RedWin", "BlueWin"
    public int MoveCount { get; set; }
    
    // Board state - сохраняем как JSON строку
    // Потому что 2D массив сложно хранить напрямую
    public string BoardState { get; set; } = default!;
}