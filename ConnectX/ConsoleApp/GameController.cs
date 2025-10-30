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
    public static GameConfiguration SetGameConfiguration()
    {
        var gameConfig = new GameConfiguration();
        
        Console.Clear();
        Console.WriteLine("=== Create Custom Game Configuration ===\n");

        bool widthIsSet = false;
        while (!widthIsSet)
        {
            Console.Write("Set Game Board Width (3-20): ");
            var input = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Width can not be null.");
                continue;
            }
            if (int.TryParse(input, out var width))
            {
                try
                {
                    gameConfig.SetWidth(width);
                    widthIsSet = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        
        // Set Height
        bool heightIsSet = false;
        while (!heightIsSet)
        {
            Console.Write("Set Game Board Height (3-20): ");
            var input = Console.ReadLine()?.Trim();
            
            // if (string.IsNullOrEmpty(input))
            // {
            //     gameConfig.BoardHeight = 6;
            //     heightIsSet = true;
            // }
            if (int.TryParse(input, out var height))
            {
                try
                {
                    gameConfig.SetHeight(height);
                    heightIsSet = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        
        // Set Win Condition
        bool winCondSet = false;
        while (!winCondSet)
        {
            Console.Write($"Set Win Condition (3-{Math.Max(gameConfig.BoardWidth, gameConfig.BoardHeight)}): ");
            var input = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(input))
            {
                gameConfig.WinCondition = 4;
                winCondSet = true;
            }
            else if (int.TryParse(input, out var cond))
            {
                try
                {
                    gameConfig.SetWinCondition(cond);
                    winCondSet = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        // Set Board Type
        Console.WriteLine("\nBoard Type:");
        Console.WriteLine("1) Rectangle");
        Console.WriteLine("2) Cylinder");
        Console.Write("Select type: ");
        var typeInput = Console.ReadLine()?.Trim();
        gameConfig.SetBoardType(typeInput == "2" ? EBoardType.Cylinder : EBoardType.Rectangle);
        
        // Set Player Types
        bool playerTypesSet = false;
        while (!playerTypesSet)
        {
            Console.WriteLine("\nSet Game Player Types:");
            Console.WriteLine("1) Human vs Human");
            Console.WriteLine("2) Human vs Computer");
            Console.WriteLine("3) Computer vs Computer");
            Console.Write("Select: ");
            
            string? input = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(input) || input == "1")
            {
                gameConfig.SetP1Type(EPlayerType.Human);
                gameConfig.SetP2Type(EPlayerType.Human);
                playerTypesSet = true;
            }
            else if (input == "2")
            {
                gameConfig.SetP1Type(EPlayerType.Human);
                gameConfig.SetP2Type(EPlayerType.Computer);
                playerTypesSet = true;
                Thread.Sleep(1500);
            }
            else if (input == "3")
            {
                gameConfig.SetP1Type(EPlayerType.Computer);
                gameConfig.SetP2Type(EPlayerType.Computer);
                playerTypesSet = true;
                Thread.Sleep(1500);
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        Console.WriteLine($"\n✓ Configuration created: {gameConfig}");
        Thread.Sleep(2000);
        
        return gameConfig;
    }

    public string GameLoop()
    {
        var config = GameBrain.GetConfiguration();
        
        while (!GameBrain.GameOver)
        {
            var board = GameBrain.GetBoard();
            Ui.DrawBoard(board);
            Ui.ShowGameInfo(config.Name, config.BoardWidth, config.BoardHeight, config.WinCondition, config.BoardType);
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());
            
            Console.Write("Enter column number (or 'x' to exit): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "x")
            {
                Console.WriteLine("\nGame quit. Returning to menu...");
                Thread.Sleep(1000);
                return "x";
            }

            if (int.TryParse(input, out var columnInput))
            {
                int column = columnInput - 1; // Convert to 0-indexed
                
                if (column >= 0 && column < config.BoardWidth)
                {
                    if (!GameBrain.ColumnIsFull(column))
                    {
                        // Process move and get row
                        int? row = GameBrain.ProcessMove(column);

                        if (row.HasValue)
                        {
                            // Piece that was just placed
                            var piece = GameBrain.IsNextPlayerX() ? ECellState.X : ECellState.O;

                            // Check for win
                            if (GameBrain.CheckWin(row.Value, column))
                            {
                                var winner = piece == ECellState.X ? ECellState.XWin : ECellState.OWin;
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
                else
                {
                    Ui.ShowError($"Please enter a number between 1 and {config.BoardWidth}.");
                }
            }
            else
            {
                Ui.ShowError("Invalid input. Please enter a column number.");
            }
        }

        // Game over - show final board and winner
        var finalBoard = GameBrain.GetBoard();
        Ui.DrawBoard(finalBoard);
        Ui.ShowWinner(GameBrain.Winner);

        return "b";
    }
}