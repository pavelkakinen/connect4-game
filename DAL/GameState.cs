using Domain;

namespace DAL;

public class GameState
{
    public string GameId { get; set; } = default!;
    public DateTime SavedAt { get; set; }
    
    public int BoardWidth { get; set; }
    public int BoardHeight { get; set; }
    public int WinCond { get; set; }
    public EBoardType BoardType { get; set; }

    public string Player1Name { get; set; } = default!;
    public string Player2Name { get; set; } = default!;
    
    public bool IsNextMoveByRed { get; set; }
    public List<List<int>> Board { get; set; } = default!;
    
    public int P1Type { get; set; } = 0;  // 0 = Human, 1 = Computer
    public int P2Type { get; set; } = 0;
}