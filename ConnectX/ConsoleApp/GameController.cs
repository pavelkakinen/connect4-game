using BLL;
using ConsoleUI;

namespace ConsoleApp;

public class GameController
{
    private GameBrain GameBrain { get; set; }

    public GameController(GameConfiguration config)
    {
        GameBrain = new GameBrain(config, "Player 1", "Player 2");
    }
    
    public string GameLoop()
    {
        var config = GameBrain.GetConfiguration();
        
        while (!GameBrain.GameOver)
        {
            var board = GameBrain.GetBoard();
            
            // Use new UI with arrow key selection
            int? selectedColumn = Ui.SelectColumn(
                board, 
                config.Name, 
                config.BoardWidth, 
                config.BoardHeight, 
                config.WinCondition, 
                config.BoardType,
                GameBrain.IsNextPlayerRed()
            );
            
            // User pressed ESC
            if (selectedColumn == null)
            {
                Console.WriteLine("\nGame quit. Returning to menu...");
                Thread.Sleep(1000);
                return "b";
            }

            int column = selectedColumn.Value;
            
            // Check if column is valid
            if (!GameBrain.ColumnIsFull(column))
            {
                // Process move and get row
                int? row = GameBrain.ProcessMove(column);

                if (row.HasValue)
                {
                    // Piece that was just placed
                    var piece = GameBrain.IsNextPlayerRed() ? ECellState.Red : ECellState.Blue;

                    // Check for win
                    if (GameBrain.CheckWin(row.Value, column))
                    {
                        var winner = piece == ECellState.Red ? ECellState.RedWin : ECellState.BlueWin;
                        GameBrain.SetGameOver(winner);
                    }
                    // Check for draw
                    else if (GameBrain.IsBoardFull())
                    {
                        GameBrain.SetGameOver(ECellState.Empty);
                    }
                    else
                    {
                        // Switch player
                        GameBrain.SwitchPlayer();
                    }
                }
            }
            else
            {
                Ui.ShowError("That column is full! Choose another.");
            }
        }

        // Game over - show final board and winner
        var finalBoard = GameBrain.GetBoard();
        Ui.DrawBoard(finalBoard);
        Ui.ShowWinner(GameBrain.Winner);

        return "b";
    }

    
    
}