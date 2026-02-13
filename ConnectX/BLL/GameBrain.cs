using Domain;
using DAL; 

namespace BLL;

public class GameBrain
{
    
    private ECellState[,] GameBoard { get; set; }
    public bool NextMoveByRed { get; private set; } = true;
    private GameConfiguration GameConfiguration { get; set; }
    private string Player1Name { get; }
    private string Player2Name { get; }
    
    public GameConfiguration GetConfiguration()
    {
        return GameConfiguration;
    }

    public GameBrain(GameConfiguration configuration,  string player1Name, string player2Name)
    {
        GameConfiguration = configuration;
        // [ROW, COLUMN] row - Y, column - X
        GameBoard = new ECellState[GameConfiguration.BoardHeight, GameConfiguration.BoardWidth];
        Player1Name = player1Name;
        Player2Name = player2Name;
    }

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardHeight, GameConfiguration.BoardWidth];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }

    public (bool success, int row) ProcessMove(int column)
    {
        if (!ColumnIsFull(column))
        {
            for (int row = GameConfiguration.BoardHeight - 1; row >= 0; row--)
            {
                if (GameBoard[row, column] == ECellState.Empty)
                {
                    GameBoard[row, column] = NextMoveByRed ? ECellState.Red : ECellState.Blue;
                    NextMoveByRed = !NextMoveByRed;
                    return (true, row);
                }
            }
        }
        return (false, -1);
    }

    private bool ColumnIsFull(int column)
    {
        return GameBoard[0, column] != ECellState.Empty;
    }

    public bool BoardIsFull()
    {
        for (var column = 0; column < GameConfiguration.BoardWidth; column++)
        {
            if (!ColumnIsFull(column))
            {
                return false;
            }
        }

        return true;
    }

    private (int row, int col) GetDirection(int index) =>
        index switch
        {
            0 => (-1, -1),
            1 => (-1, 0),
            2 => (-1, 1),
            3 => (0, 1),
            _ => (0, 0)
        };

    private int NormalizeColumn(int column)
    {
        if (GameConfiguration.BoardType == EBoardType.Cylinder)
        {
            return (column % GameConfiguration.BoardWidth + GameConfiguration.BoardWidth) % GameConfiguration.BoardWidth;
        }

        return column;
    }

    private bool ValidateCoordinates(int row, int col)
    {
        if (row < 0 || row >= GameConfiguration.BoardHeight) return false;

        if (GameConfiguration.BoardType == EBoardType.Cylinder)
            return true;

        if (col < 0 || col >= GameConfiguration.BoardWidth) return false;
        return true;
    }

    private List<(int row, int col)> GetCellsInDirection(int row, int column, int dirY, int dirX)
    {
        var cells = new List<(int row, int col)>();

        var nextY = row + dirY;
        var nextX = NormalizeColumn(column +  dirX);

        while (ValidateCoordinates(nextY, nextX) &&
               GameBoard[nextY, NormalizeColumn(nextX)] == GameBoard[row, column])
        {
            cells.Add((nextY, nextX));
            nextY += dirY;
            nextX = NormalizeColumn(nextX + dirX);
        }

        return cells;
    }

    public (ECellState winner, List<(int row, int col)> winningCells) CheckWin(int row, int column)
    {
        for (int index = 0; index < 4; index++)
        {
            var winningCells = new List<(int row, int col)>();
            var (dirY,  dirX) = GetDirection(index);
            
            var cells1 = GetCellsInDirection(row, column, dirY,dirX);
            var cells2 = GetCellsInDirection(row, column, -dirY, -dirX);

            winningCells.AddRange(cells1);
            winningCells.AddRange(cells2);
            
            winningCells.Add((row, column));
            if (winningCells.Count >= GameConfiguration.WinCondition)
            {
                return (GameBoard[row, column], winningCells);
            }
        }

        return (ECellState.Empty, null);
    }
    
    public (ECellState winner, List<(int row, int col)> winningCells) CheckWin()
    {
        // Check every non-empty cell
        for (int row = 0; row < GameBoard.GetLength(0); row++)
        {
            for (int col = 0; col < GameBoard.GetLength(1); col++)
            {
                var cellState = GameBoard[row, col];
            
                if (cellState != ECellState.Empty)
                {
                    var result = CheckWin(row, col);
                
                    if (result.winner != ECellState.Empty)
                    {
                        return result; // Found a winner!
                    }
                }
            }
        }
    
        // No winner found
        return (ECellState.Empty, new List<(int row, int col)>());
    }

    public GameState GetGameState(string? gameId = null) =>
    new GameState()
    {
        GameId = gameId ?? "",
        SavedAt = DateTime.Now,
        BoardWidth = GameConfiguration.BoardWidth,
        BoardHeight = GameConfiguration.BoardHeight,
        WinCond = GameConfiguration.WinCondition,
        BoardType = GameConfiguration.BoardType,
        Player1Name = Player1Name,
        Player2Name = Player2Name,
        IsNextMoveByRed = NextMoveByRed,
        Board = SerializeBoard(),
        P1Type = (int)GameConfiguration.P1Type,
        P2Type = (int)GameConfiguration.P2Type
    };
    
    
    private List<List<int>> SerializeBoard()
    {
        var list = new List<List<int>>();
        
        for (int row = 0; row < GameConfiguration.BoardHeight; row++)
        {
            var rowList = new List<int>();
            for (int col = 0; col < GameConfiguration.BoardWidth; col++)
            {
                rowList.Add((int)GameBoard[row, col]);
            }
            list.Add(rowList);
        }
        
        return list;
    }
    
    public void LoadFromGameState(GameState state)
    {
        DeserializeBoard(state.Board);

        NextMoveByRed = state.IsNextMoveByRed;
        GameConfiguration.SetP1Type((EPlayerType)state.P1Type);
        GameConfiguration.SetP2Type((EPlayerType)state.P2Type);
    }

    private void DeserializeBoard(List<List<int>> board)
    {
        for (int row = 0; row < board.Count; row++)
        {
            for (int col = 0; col < board[row].Count; col++)
            {
                GameBoard[row, col] = (ECellState)board[row][col];
            }
        }
    }

    public bool IsGameFinished()
    {
        if (BoardIsFull()) return true;
        var (winner, _) = CheckWin();
        return winner != ECellState.Empty;
    }

}