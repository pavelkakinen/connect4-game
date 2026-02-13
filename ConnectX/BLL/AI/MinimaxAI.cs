using Domain;

namespace BLL.AI;

/// <summary>
/// AI player using Minimax algorithm with Alpha-Beta pruning
/// </summary>
public class MinimaxAI : IAIPlayer
{
    private readonly int _maxDepth;

    public MinimaxAI(int maxDepth = 6)
    {
        _maxDepth = maxDepth;
    }

    public int GetBestMove(ECellState[,] board, bool isRedPlayer, GameConfiguration config)
    {
        var aiColor = isRedPlayer ? ECellState.Red : ECellState.Blue;

        var winningMove = AIHelper.FindWinningMove(board, aiColor, config);
        if (winningMove.HasValue)
            return winningMove.Value;

        var blockingMove = AIHelper.FindBlockingMove(board, aiColor, config);
        if (blockingMove.HasValue)
            return blockingMove.Value;

        return FindBestMinimaxMove(board, aiColor, config);
    }

    private int FindBestMinimaxMove(ECellState[,] board, ECellState aiColor, GameConfiguration config)
    {
        int bestMove = -1;
        int bestScore = int.MinValue;

        var availableColumns = AIHelper.GetAvailableColumns(board);

        foreach (var col in availableColumns)
        {
            var newBoard = AIHelper.MakeMove(board, col, aiColor);
            int score = Minimax(newBoard, _maxDepth - 1, int.MinValue, int.MaxValue, false, aiColor, config);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = col;
            }
        }

        return bestMove;
    }

    /// <summary>
    /// Minimax algorithm with Alpha-Beta pruning
    /// </summary>
    private int Minimax(ECellState[,] board, int depth, int alpha, int beta, bool isMaximizing, ECellState aiColor, GameConfiguration config)
    {
        var winner = CheckWinner(board, config);
        if (winner == aiColor) return 10000 + depth;
        if (winner != ECellState.Empty && winner != aiColor) return -10000 - depth;
        if (AIHelper.IsBoardFull(board)) return 0;

        if (depth == 0) return EvaluateBoard(board, aiColor, config);

        var color = isMaximizing ? aiColor : AIHelper.GetOpponentColor(aiColor);
        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        var availableColumns = AIHelper.GetAvailableColumns(board);

        foreach (var col in availableColumns)
        {
            var newBoard = AIHelper.MakeMove(board, col, color);
            int score = Minimax(newBoard, depth - 1, alpha, beta, !isMaximizing, aiColor, config);

            if (isMaximizing)
            {
                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, score);
            }
            else
            {
                bestScore = Math.Min(bestScore, score);
                beta = Math.Min(beta, score);
            }

            if (beta <= alpha)
                break;
        }

        return bestScore;
    }

    /// <summary>
    /// Evaluate board position (heuristic)
    /// </summary>
    private int EvaluateBoard(ECellState[,] board, ECellState aiColor, GameConfiguration config)
    {
        int score = 0;
        var opponentColor = AIHelper.GetOpponentColor(aiColor);

        score += EvaluateLines(board, aiColor, opponentColor, config);
        score += EvaluateCenterControl(board, aiColor);

        return score;
    }

    private int EvaluateLines(ECellState[,] board, ECellState aiColor, ECellState opponentColor, GameConfiguration config)
    {
        int score = 0;
        int height = board.GetLength(0);
        int width = board.GetLength(1);

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (col + config.WinCondition <= width)
                    score += EvaluateLine(board, row, col, 0, 1, aiColor, opponentColor, config);

                if (row + config.WinCondition <= height)
                    score += EvaluateLine(board, row, col, 1, 0, aiColor, opponentColor, config);

                if (row + config.WinCondition <= height && col + config.WinCondition <= width)
                    score += EvaluateLine(board, row, col, 1, 1, aiColor, opponentColor, config);

                if (row + config.WinCondition <= height && col - config.WinCondition >= -1)
                    score += EvaluateLine(board, row, col, 1, -1, aiColor, opponentColor, config);
            }
        }

        return score;
    }

    private int EvaluateLine(ECellState[,] board, int startRow, int startCol, int dRow, int dCol,
        ECellState aiColor, ECellState opponentColor, GameConfiguration config)
    {
        int aiCount = 0;
        int opponentCount = 0;
        int emptyCount = 0;

        for (int i = 0; i < config.WinCondition; i++)
        {
            int row = startRow + i * dRow;
            int col = startCol + i * dCol;

            if (row < 0 || row >= board.GetLength(0) || col < 0 || col >= board.GetLength(1))
                return 0;

            if (board[row, col] == aiColor) aiCount++;
            else if (board[row, col] == opponentColor) opponentCount++;
            else emptyCount++;
        }

        if (aiCount > 0 && opponentCount > 0) return 0;

        if (aiCount > 0)
        {
            return (int)Math.Pow(10, aiCount);
        }
        else if (opponentCount > 0)
        {
            return -(int)Math.Pow(10, opponentCount);
        }

        return 0;
    }

    private int EvaluateCenterControl(ECellState[,] board, ECellState aiColor)
    {
        int score = 0;
        int centerCol = board.GetLength(1) / 2;

        for (int row = 0; row < board.GetLength(0); row++)
        {
            if (board[row, centerCol] == aiColor)
                score += 3;
        }

        return score;
    }

    /// <summary>
    /// Check winner on board (simplified)
    /// </summary>
    private ECellState CheckWinner(ECellState[,] board, GameConfiguration config)
    {
        int height = board.GetLength(0);
        int width = board.GetLength(1);

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                var cell = board[row, col];
                if (cell == ECellState.Empty) continue;

                if (AIHelper.CheckWin(board, row, col, cell, config))
                {
                    return cell;
                }
            }
        }

        return ECellState.Empty;
    }
}
