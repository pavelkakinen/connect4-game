namespace BLL;

public class GameBrain
{
    private ECellState[,] GameBoard { get; set; }
    private GameConfiguration GameConfiguration { get; set; }
    public string Player1Name { get; set; }
    public string Player2Name { get; set; }

    public bool NextMoveByRed { get; set; } = true;
    public bool GameOver { get; set; } = false;
    public ECellState Winner { get; set; } = ECellState.Empty;
    public int MoveCount { get; set; } = 0;
    
    public GameBrain(GameConfiguration configuration, string player1Name, string player2Name)
    {
        GameConfiguration = configuration;
        GameConfiguration.Validate();
        Player1Name = player1Name;
        Player2Name = player2Name;
        // Board is [row, column] - rows are Y axis, columns are X axis
        GameBoard = new ECellState[configuration.BoardHeight, configuration.BoardWidth];
    }

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardHeight, GameConfiguration.BoardWidth];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }

    public GameConfiguration GetConfiguration() => GameConfiguration;

    public bool IsNextPlayerX() => NextMoveByRed;

    public bool IsColumnFull(int column)
    {
        if (column < 0 || column >= GameConfiguration.BoardWidth)
            return true;
        
        return GameBoard[0, column] != ECellState.Empty;
    }

    // Returns the row where piece was placed, or null if move failed
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
        
        // Find the lowest empty row in the column
        for (int row = GameConfiguration.BoardHeight - 1; row >= 0; row--)
        {
            if (GameBoard[row, column] == ECellState.Empty)
            {
                GameBoard[row, column] = NextMoveByRed ? ECellState.Red : ECellState.Blue;
                MoveCount++;
                return row;
            }
        }
        
        return null; // Column is full
    }
    public void SwitchPlayer()
    {
        NextMoveByRed = !NextMoveByRed;
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
            if (!IsColumnFull(col))
                return false;
        }
        return true;
    }

    public void SetGameOver(ECellState winner)
    {
        GameOver = true;
        Winner = winner;
    }

    public string GetCurrentPlayerName()
    {
        return NextMoveByRed ? Player1Name : Player2Name;
    }

    // Method to restore board state (for loading saved games)
    public void SetBoard(ECellState[,] board)
    {
        if (board.GetLength(0) == GameConfiguration.BoardHeight && 
            board.GetLength(1) == GameConfiguration.BoardWidth)
        {
            GameBoard = board;
        }
        else
        {
            throw new ArgumentException("Board dimensions don't match configuration");
        }
    }
}