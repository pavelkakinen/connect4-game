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

    public static GameConfiguration? SetGameConfiguration()
    {
        var gameConfig = new GameConfiguration();
        
        Console.Clear();
        Console.WriteLine("=== Create Custom Game Configuration ===\n");

        // Get name
        Console.Write("Game name: ");
        var nameInput = Console.ReadLine()?.Trim().ToLower();
        if (nameInput == "b" || nameInput == "x")
            return null;
        if (!string.IsNullOrEmpty(nameInput))
            gameConfig.Name = nameInput;

        // Set Width
        bool widthIsSet = false;
        while (!widthIsSet)
        {
            Console.Write("Set Game Board Width (3-20): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "b" || input == "x")
                return null;
            
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Error: Value cannot be empty.");
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
            else
            {
                Console.WriteLine("Error: Please enter a valid number.");
            }
        }
        
        // Set Height
        bool heightIsSet = false;
        while (!heightIsSet)
        {
            Console.Write("Set Game Board Height (3-20): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "b" || input == "x")
                return null;
            
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Error: Value cannot be empty.");
                continue;
            }
            
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
            else
            {
                Console.WriteLine("Error: Please enter a valid number.");
            }
        }
        
        // Set Win Condition
        bool winCondSet = false;
        while (!winCondSet)
        {
            Console.Write($"Set Win Condition (3-{Math.Max(gameConfig.BoardWidth, gameConfig.BoardHeight)}): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "b" || input == "x")
                return null;
            
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Error: Value cannot be empty.");
                continue;
            }
            
            if (int.TryParse(input, out var cond))
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
            else
            {
                Console.WriteLine("Error: Please enter a valid number.");
            }
        }

        // Set Board Type
        bool boardTypeSet = false;
        while (!boardTypeSet)
        {
            Console.WriteLine("\nBoard Type:");
            Console.WriteLine("1) Rectangle");
            Console.WriteLine("2) Cylinder");
            Console.Write("Select type: ");
            var typeInput = Console.ReadLine()?.Trim().ToLower();
            
            if (typeInput == "b" || typeInput == "x")
                return null;
            
            if (string.IsNullOrEmpty(typeInput))
            {
                Console.WriteLine("Error: Value cannot be empty.");
                continue;
            }
            
            if (typeInput == "1")
            {
                gameConfig.SetBoardType(EBoardType.Rectangle);
                boardTypeSet = true;
            }
            else if (typeInput == "2")
            {
                gameConfig.SetBoardType(EBoardType.Cylinder);
                boardTypeSet = true;
            }
            else
            {
                Console.WriteLine("Error: Please enter 1 or 2.");
            }
        }
        
        // Set Player Types
        bool playerTypesSet = false;
        while (!playerTypesSet)
        {
            Console.WriteLine("\nSet Game Player Types:");
            Console.WriteLine("1) Human vs Human");
            Console.WriteLine("2) Human vs Computer (Coming in HW05)");
            Console.WriteLine("3) Computer vs Computer (Coming in HW05)");
            Console.Write("Select: ");
            
            string? input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "b" || input == "x")
                return null;
            
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Error: Value cannot be empty.");
                continue;
            }
            
            if (input == "1")
            {
                gameConfig.SetP1Type(EPlayerType.Human);
                gameConfig.SetP2Type(EPlayerType.Human);
                playerTypesSet = true;
            }
            else if (input == "2")
            {
                Console.WriteLine("\n⚠️  AI not yet implemented. Using Human vs Human.");
                gameConfig.SetP1Type(EPlayerType.Human);
                gameConfig.SetP2Type(EPlayerType.Human);
                playerTypesSet = true;
                Thread.Sleep(1500);
            }
            else if (input == "3")
            {
                Console.WriteLine("\n⚠️  AI not yet implemented. Using Human vs Human.");
                gameConfig.SetP1Type(EPlayerType.Human);
                gameConfig.SetP2Type(EPlayerType.Human);
                playerTypesSet = true;
                Thread.Sleep(1500);
            }
            else
            {
                Console.WriteLine("Error: Please enter 1, 2, or 3.");
            }
        }

        Console.WriteLine($"\n✓ Configuration created: {gameConfig}");
        Thread.Sleep(1500);
        
        return gameConfig;
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
                GameBrain.IsNextPlayerX()
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
            if (!GameBrain.IsColumnFull(column))
            {
                // Process move and get row
                int? row = GameBrain.ProcessMove(column);

                if (row.HasValue)
                {
                    // Piece that was just placed
                    var piece = GameBrain.IsNextPlayerX() ? ECellState.Red : ECellState.Blue;

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