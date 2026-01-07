using Domain;

namespace BLL;

public class GameConfiguration
{
    private string Name { get; set; } = "Custom Game";
    public int BoardWidth { get; set; } = 7;
    public int BoardHeight { get; set; } = 6;
    public int WinCondition { get; set; } = 4;
    public EBoardType BoardType { get; set; } = EBoardType.Rectangle;

    public EPlayerType P1Type { get; private set; } = EPlayerType.Human;
    public EPlayerType P2Type { get; private set; } = EPlayerType.Human;
    

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

    public override string ToString()
    {
        return $"{Name}: {BoardWidth}x{BoardHeight}, Win: {WinCondition}, Type: {BoardType}, P1: {P1Type},  P2: {P2Type}";
    }
}