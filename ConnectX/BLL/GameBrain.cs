namespace BLL;

public class GameBrain
{
    private ECellState[,] GameBoard { get; set; }
    private GameConfiguration GameConfiguration { get; set; }
    private string Player1Name { get; set; }
    private string Player2Name { get; set; }

    private bool NextMoveByX { get; set; } = true;
    public bool GameOver { get; set; } = false;
    public ECellState Winner { get; set; } = ECellState.Empty;
    
    public GameBrain(GameConfiguration configuration, string player1Name, string player2Name)
    {
        GameConfiguration = configuration;
        GameConfiguration.Validate();
        Player1Name = player1Name;
        Player2Name = player2Name;
        GameBoard = new ECellState[configuration.BoardHeight, configuration.BoardWidth];
    }

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardHeight, GameConfiguration.BoardWidth];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }

    public GameConfiguration GetConfiguration() => GameConfiguration;

    public bool IsNextPlayerX() => NextMoveByX;

    public bool ColumnIsFull(int column)
    {
        if (column < 0 || column >= GameConfiguration.BoardWidth)
            return true;
        
        return GameBoard[0, column] != ECellState.Empty;
    }

    public int? ProcessMove(int column)
    {
        if (column < 0 || column >= GameConfiguration.BoardWidth)
        {
            return null;
        }

        if (GameOver)
        {
            return null;
        }
        
        for (int row = GameConfiguration.BoardHeight - 1; row >= 0; row--)
        {
            if (GameBoard[row, column] == ECellState.Empty)
            {
                GameBoard[row, column] = NextMoveByX ? ECellState.Red : ECellState.Blue;
                return row;
            }
        }
        
        return null;
    }

    public void SwitchPlayer()
    {
        NextMoveByX = !NextMoveByX;
    }

    // Check if the last move resulted in a win
    public bool CheckWin(int row, int column)
    {
        var player = GameBoard[row, column];
        
        if (player == ECellState.Empty)
            return false;

        // Check all 4 directions: horizontal, vertical, diagonal-right, diagonal-left
        if (CheckDirection(row, column, 0, 1, player)) return true;  // Horizontal
        if (CheckDirection(row, column, 1, 0, player)) return true;  // Vertical
        if (CheckDirection(row, column, 1, 1, player)) return true;  // Diagonal down-right
        if (CheckDirection(row, column, 1, -1, player)) return true; // Diagonal down-left

        return false;
    }

    private bool CheckDirection(int row, int col, int dRow, int dCol, ECellState player)
    {
        int count = 1; // Count the piece we just placed

        // Check in positive direction
        count += CountInDirection(row, col, dRow, dCol, player);
        
        // Check in negative direction
        count += CountInDirection(row, col, -dRow, -dCol, player);

        return count >= GameConfiguration.WinCondition;
    }

    private int CountInDirection(int row, int col, int dRow, int dCol, ECellState player)
    {
        int count = 0;
        int currentRow = row + dRow;
        int currentCol = col + dCol;

        while (true)
        {
            // Handle cylinder wrapping for columns
            if (GameConfiguration.BoardType == EBoardType.Cylinder)
            {
                currentCol = (currentCol + GameConfiguration.BoardWidth) % GameConfiguration.BoardWidth;
                if (currentCol < 0)
                    currentCol += GameConfiguration.BoardWidth;
            }

            // Check bounds
            if (currentRow < 0 || currentRow >= GameConfiguration.BoardHeight)
                break;

            if (GameConfiguration.BoardType == EBoardType.Rectangle)
            {
                if (currentCol < 0 || currentCol >= GameConfiguration.BoardWidth)
                    break;
            }

            // Check if piece matches
            if (GameBoard[currentRow, currentCol] == player)
            {
                count++;
                currentRow += dRow;
                currentCol += dCol;
            }
            else
            {
                break;
            }
        }

        return count;
    }

    public bool IsBoardFull()
    {
        for (int col = 0; col < GameConfiguration.BoardWidth; col++)
        {
            if (!ColumnIsFull(col))
                return false;
        }
        return true;
    }

    public void SetGameOver(ECellState winner)
    {
        GameOver = true;
        Winner = winner;
    }
}