namespace BLL;

public class GameConfiguration
{
    public string Name { get; set; } = "Custom Game";
    public int BoardWidth { get; set; } = 7;
    public int BoardHeight { get; set; } = 6;
    public int WinCondition { get; set; } = 4;
    public EBoardType BoardType { get; set; } = EBoardType.Rectangle;
    
    public EPlayerType P1Type { get; set; } = EPlayerType.Human;
    public EPlayerType P2Type { get; set; } = EPlayerType.Human;

    // Predefined configurations
    public static GameConfiguration Classic() => 
        new GameConfiguration 
        { 
            Name = "Classic Connect 4", 
            BoardWidth = 7, 
            BoardHeight = 6, 
            WinCondition = 4,
            BoardType = EBoardType.Rectangle
        };

    public static GameConfiguration Connect5() => 
        new GameConfiguration 
        { 
            Name = "Connect 5", 
            BoardWidth = 9, 
            BoardHeight = 7, 
            WinCondition = 5,
            BoardType = EBoardType.Rectangle
        };

    public static GameConfiguration Connect3() => 
        new GameConfiguration 
        { 
            Name = "Connect 3", 
            BoardWidth = 5, 
            BoardHeight = 4, 
            WinCondition = 3,
            BoardType = EBoardType.Rectangle
        };

    public static GameConfiguration Connect4Cylinder() => 
        new GameConfiguration 
        { 
            Name = "Connect 4 Cylinder", 
            BoardWidth = 7, 
            BoardHeight = 6, 
            WinCondition = 4,
            BoardType = EBoardType.Cylinder
        };
    
    public void SetWidth(int x)
    {
        if (x < 3 || x > 20)
        {
            throw new ArgumentException("Width must be between 3 and 20");
        }
        BoardWidth = x;
    }
    
    public void SetHeight(int y)
    {
        if (y < 3 || y > 20)
        {
            throw new ArgumentException("Height must be between 3 and 20");
        }
        BoardHeight = y;
    }

    public void SetWinCondition(int winCondition)
    {
        if (winCondition < 3)
        {
            throw new ArgumentException("Win condition must be at least 3");
        }
        
        if (winCondition > Math.Max(BoardHeight, BoardWidth))
        {
            throw new ArgumentException("Win condition can't be greater than board dimensions");
        }
        WinCondition = winCondition;
    }

    public void SetP1Type(EPlayerType p1Type)
    {
        P1Type = p1Type;
    }

    public void SetP2Type(EPlayerType p2Type)
    {
        P2Type = p2Type;
    }

    public void SetBoardType(EBoardType boardType)
    {
        BoardType = boardType;
    }

    public void Validate()
    {
        if (BoardWidth < 3 || BoardWidth > 20)
            throw new ArgumentException("Board width must be between 3 and 20");
        
        if (BoardHeight < 3 || BoardHeight > 20)
            throw new ArgumentException("Board height must be between 3 and 20");
        
        if (WinCondition < 3)
            throw new ArgumentException("Win condition must be at least 3");
        
        if (WinCondition > Math.Max(BoardWidth, BoardHeight))
            throw new ArgumentException("Win condition cannot exceed board dimensions");
    }

    public override string ToString()
    {
        return $"{Name}: {BoardWidth}x{BoardHeight}, Win: {WinCondition}, Type: {BoardType}";
    }
}