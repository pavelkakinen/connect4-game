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
        _maxDepth = maxDepth; // Depth of search (how many moves ahead to think)
    }
    
    public int GetBestMove(ECellState[,] board, bool isRedPlayer, GameConfiguration config)
    {
        var aiColor = isRedPlayer ? ECellState.Red : ECellState.Blue;
        
        // 1. First - check if there's an immediate winning move!
        var winningMove = AIHelper.FindWinningMove(board, aiColor, config);
        if (winningMove.HasValue)
            return winningMove.Value; // Win immediately!
        
        // 2. Second - check if we need to block opponent!
        var blockingMove = AIHelper.FindBlockingMove(board, aiColor, config);
        if (blockingMove.HasValue)
            return blockingMove.Value; // Block!
        
        // 3. Third - use Minimax for best move
        int bestMove = -1;
        int bestScore = int.MinValue;
        
        var availableColumns = AIHelper.GetAvailableColumns(board);
        
        foreach (var col in availableColumns)
        {
            var newBoard = AIHelper.MakeMove(board, col, aiColor);
            
            // Evaluate position (opponent will minimize our score)
            int score = Minimax(newBoard, _maxDepth - 1, int.MinValue, int.MaxValue, false, aiColor, config);
            
            // If this move is better → remember it
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
        // Check game end
        var winner = CheckWinner(board, config);
        if (winner == aiColor) return 10000 + depth; // AI won (faster = better)
        if (winner != ECellState.Empty && winner != aiColor) return -10000 - depth; // AI lost
        if (AIHelper.IsBoardFull(board)) return 0; // Draw
        
        // Reached maximum depth → evaluate position
        if (depth == 0) return EvaluateBoard(board, aiColor, config);
        
        if (isMaximizing)
        {
            // AI move - maximize score
            int maxScore = int.MinValue;
            
            var availableColumns = AIHelper.GetAvailableColumns(board);
            
            foreach (var col in availableColumns)
            {
                var newBoard = AIHelper.MakeMove(board, col, aiColor);
                int score = Minimax(newBoard, depth - 1, alpha, beta, false, aiColor, config);
                
                maxScore = Math.Max(maxScore, score);
                alpha = Math.Max(alpha, score);
                
                // Alpha-Beta pruning
                if (beta <= alpha)
                    break;
            }
            
            return maxScore;
        }
        else
        {
            // Opponent move - minimize our score
            int minScore = int.MaxValue;
            var opponentColor = AIHelper.GetOpponentColor(aiColor);
            
            var availableColumns = AIHelper.GetAvailableColumns(board);
            
            foreach (var col in availableColumns)
            {
                var newBoard = AIHelper.MakeMove(board, col, opponentColor);
                int score = Minimax(newBoard, depth - 1, alpha, beta, true, aiColor, config);
                
                minScore = Math.Min(minScore, score);
                beta = Math.Min(beta, score);
                
                // Alpha-Beta pruning
                if (beta <= alpha)
                    break;
            }
            
            return minScore;
        }
    }
    
    /// <summary>
    /// Evaluate board position (heuristic)
    /// </summary>
    private int EvaluateBoard(ECellState[,] board, ECellState aiColor, GameConfiguration config)
    {
        int score = 0;
        var opponentColor = AIHelper.GetOpponentColor(aiColor);
        
        // Evaluate all possible lines (horizontal, vertical, diagonals)
        score += EvaluateLines(board, aiColor, opponentColor, config);
        
        // Bonus for center control (strategically important)
        score += EvaluateCenterControl(board, aiColor);
        
        return score;
    }
    
    private int EvaluateLines(ECellState[,] board, ECellState aiColor, ECellState opponentColor, GameConfiguration config)
    {
        int score = 0;
        int height = board.GetLength(0);
        int width = board.GetLength(1);
        
        // Check all possible lines of length WinCondition
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // Horizontal
                if (col + config.WinCondition <= width)
                    score += EvaluateLine(board, row, col, 0, 1, aiColor, opponentColor, config);
                
                // Vertical
                if (row + config.WinCondition <= height)
                    score += EvaluateLine(board, row, col, 1, 0, aiColor, opponentColor, config);
                
                // Diagonal ↘
                if (row + config.WinCondition <= height && col + config.WinCondition <= width)
                    score += EvaluateLine(board, row, col, 1, 1, aiColor, opponentColor, config);
                
                // Diagonal ↙
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
        
        // If line has both players' pieces → blocked
        if (aiCount > 0 && opponentCount > 0) return 0;
        
        // Evaluate line
        if (aiCount > 0)
        {
            // Line is useful for AI
            return (int)Math.Pow(10, aiCount);
        }
        else if (opponentCount > 0)
        {
            // Line is dangerous (opponent can win)
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
        
        // Check all positions for potential win
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
