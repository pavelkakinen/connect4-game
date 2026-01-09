using Domain;

namespace BLL.AI;

/// <summary>
/// Interface for AI player
/// </summary>
public interface IAIPlayer
{
    /// <summary>
    /// Get best move for current board state
    /// </summary>
    /// <param name="board">Current game board</param>
    /// <param name="isRedPlayer">True if AI plays as Red</param>
    /// <param name="config">Game configuration</param>
    /// <returns>Column index for best move</returns>
    int GetBestMove(ECellState[,] board, bool isRedPlayer, GameConfiguration config);
}