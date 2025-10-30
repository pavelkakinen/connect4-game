namespace BLL;

public class GameConfiguration
{
    public int BoardWidth { get; set; } = 5;
    public int BoardHeight { get; set; } = 5;
    public int WinCondition { get; set; } = 3;
    
    public EPlayerType P1Type { get; set; } = EPlayerType.Human;
    public EPlayerType P2Type { get; set; } = EPlayerType.Human;

    public void setWidth(int x)
    {
        if (x < 3)
        {
            throw new ArgumentException("Width must be 3 or more");
        }
        BoardWidth = x;
    }
    
    
    public void setHeight(int y)
    {
        if (y < 3)
        {
            throw new ArgumentException("Width must be 3 or more");
        }
        BoardHeight = y;
    }

    public void setWinCondition(int winCondition)
    {
        if (winCondition > BoardHeight || winCondition > BoardWidth)
        {
            throw new ArgumentException("WinCondition can't be greater than Board Height and Board Width");
        }
        WinCondition = winCondition;
    }

    public void setP1Type(EPlayerType p1Type)
    {
        P1Type = p1Type;
    }

    public void setP2Type(EPlayerType p2Type)
    {
        P2Type = p2Type;
    }
}