using Domain;

namespace BLL.AI;

/// <summary>
/// Helper methods for AI calculations
/// </summary>
public static class AIHelper
{
    /// <summary>
    /// Clone board to create independent copy
    /// </summary>
    public static ECellState[,] CloneBoard(ECellState[,] board)
    {
        return (ECellState[,])board.Clone();
    }

    /// <summary>
    /// Check if column is full
    /// </summary>
    public static bool IsColumnFull(ECellState[,] board, int col)
    {
        if (col < 0 || col >= board.GetLength(1))
            return true;

        return board[0, col] != ECellState.Empty;
    }

    /// <summary>
    /// Check if entire board is full
    /// </summary>
    public static bool IsBoardFull(ECellState[,] board)
    {
        for (int col = 0; col < board.GetLength(1); col++)
        {
            if (!IsColumnFull(board, col))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Get available columns (not full)
    /// </summary>
    public static List<int> GetAvailableColumns(ECellState[,] board)
    {
        var availableColumns = new List<int>();

        for (int col = 0; col < board.GetLength(1); col++)
        {
            if (!IsColumnFull(board, col))
            {
                availableColumns.Add(col);
            }
        }

        return availableColumns;
    }

    /// <summary>
    /// Make a move on the board (returns new board, doesn't modify original)
    /// </summary>
    public static ECellState[,] MakeMove(ECellState[,] board, int col, ECellState color)
    {
        var newBoard = CloneBoard(board);

        for (int row = board.GetLength(0) - 1; row >= 0; row--)
        {
            if (newBoard[row, col] == ECellState.Empty)
            {
                newBoard[row, col] = color;
                break;
            }
        }

        return newBoard;
    }

    /// <summary>
    /// Get row where piece would land in column
    /// </summary>
    public static int GetDropRow(ECellState[,] board, int col)
    {
        for (int row = board.GetLength(0) - 1; row >= 0; row--)
        {
            if (board[row, col] == ECellState.Empty)
            {
                return row;
            }
        }
        return -1;
    }

    /// <summary>
    /// Check for immediate winning move
    /// </summary>
    public static int? FindWinningMove(ECellState[,] board, ECellState color, GameConfiguration config)
    {
        for (int col = 0; col < board.GetLength(1); col++)
        {
            if (IsColumnFull(board, col))
                continue;

            int row = GetDropRow(board, col);
            if (row == -1)
                continue;

            var testBoard = MakeMove(board, col, color);

            if (CheckWin(testBoard, row, col, color, config))
            {
                return col;
            }
        }

        return null;
    }

    /// <summary>
    /// Check for immediate blocking move (opponent about to win)
    /// </summary>
    public static int? FindBlockingMove(ECellState[,] board, ECellState aiColor, GameConfiguration config)
    {
        var opponentColor = GetOpponentColor(aiColor);
        return FindWinningMove(board, opponentColor, config);
    }

    /// <summary>
    /// Simple win check
    /// </summary>
    public static bool CheckWin(ECellState[,] board, int row, int col, ECellState color, GameConfiguration config)
    {
        int height = board.GetLength(0);
        int width = board.GetLength(1);
        int winCond = config.WinCondition;
        bool isCylinder = config.BoardType == EBoardType.Cylinder;

        // Horizontal
        int count = 1 + CountInDirection(board, row, col, 0, -1, color, width, height)
                      + CountInDirection(board, row, col, 0, 1, color, width, height);
        if (count >= winCond) return true;

        // Horizontal (cylinder wrap)
        if (isCylinder && count < winCond)
        {
            count = 1 + CountHorizontalCylinder(board, row, col, color, config);
            if (count >= winCond) return true;
        }

        // Vertical
        count = 1 + CountInDirection(board, row, col, -1, 0, color, width, height)
                  + CountInDirection(board, row, col, 1, 0, color, width, height);
        if (count >= winCond) return true;

        // Diagonal right-down
        count = 1 + CountInDirection(board, row, col, -1, -1, color, width, height)
                  + CountInDirection(board, row, col, 1, 1, color, width, height);
        if (count >= winCond) return true;

        // Diagonal left-down
        count = 1 + CountInDirection(board, row, col, -1, 1, color, width, height)
                  + CountInDirection(board, row, col, 1, -1, color, width, height);
        if (count >= winCond) return true;

        return false;
    }

    private static int CountInDirection(ECellState[,] board, int row, int col, int dRow, int dCol,
        ECellState color, int width, int height)
    {
        int count = 0;
        int r = row + dRow;
        int c = col + dCol;

        while (r >= 0 && r < height && c >= 0 && c < width && board[r, c] == color)
        {
            count++;
            r += dRow;
            c += dCol;
        }

        return count;
    }

    private static int CountHorizontalCylinder(ECellState[,] board, int row, int col,
        ECellState color, GameConfiguration config)
    {
        int width = board.GetLength(1);
        int count = 0;

        for (int i = 1; i < config.WinCondition; i++)
        {
            int c = (col - i + width) % width;
            if (board[row, c] == color) count++;
            else break;
        }
        for (int i = 1; i < config.WinCondition; i++)
        {
            int c = (col + i) % width;
            if (board[row, c] == color) count++;
            else break;
        }

        return count;
    }

    /// <summary>
    /// Get opposite color
    /// </summary>
    public static ECellState GetOpponentColor(ECellState color)
    {
        return color == ECellState.Red ? ECellState.Blue : ECellState.Red;
    }
}
