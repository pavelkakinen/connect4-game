using BLL;

namespace ConsoleUI;

public static class Ui
{
    public static int? SelectColumn(ECellState[,] gameBoard, string gameName, int width, int height, int winCondition, EBoardType boardType, bool isNextPlayerX)
    {
        int column = 0;
        string empty = "   ";
        string wanted = "⬇️   ";
        
        while (true)
        {
            Console.Clear();
            
            // Show game info
            ShowGameInfo(gameName, width, height, winCondition, boardType);
            ShowNextPlayer(isNextPlayerX);
            Console.WriteLine();
            
            // Show column selector
            for (int col = 0; col < width; col++)
            {
                if (col == column)
                {
                    Console.Write(wanted);
                }
                else
                {
                    Console.Write(empty);
                }
            }
            Console.WriteLine();
            
            // Show board
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Console.Write(GetCellRepresentation(gameBoard[row, col]));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Use ← → arrows to select column, ENTER to drop piece, ESC to quit");
            
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    column = (column == 0) ? width - 1 : column - 1;
                    break;
                case ConsoleKey.RightArrow:
                    column = (column == width - 1) ? 0 : column + 1;
                    break;
                case ConsoleKey.Enter:
                    return column; // Return selected column
                case ConsoleKey.Escape:
                    return null; // User wants to quit
            }
        }
    }

    public static void DrawBoard(ECellState[,] gameBoard)
    {
        int height = gameBoard.GetLength(0);
        int width = gameBoard.GetLength(1);
        
        Console.Clear();
        
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Console.Write(GetCellRepresentation(gameBoard[row, col]));
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public static void ShowWinner(ECellState winner)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        if (winner == ECellState.RedWin)
        {
            Console.WriteLine("       PLAYER 🔴 WINS!");
        }
        else if (winner == ECellState.BlueWin)
        {
            Console.WriteLine("       PLAYER 🔵 WINS!");
        }
        else
        {
            Console.WriteLine("         DRAW GAME!");
        }
        Console.WriteLine("================================");
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public static void ShowGameInfo(string gameName, int width, int height, int winCondition, EBoardType boardType)
    {
        Console.WriteLine($"Game: {gameName} | Board: {width}x{height} | Win: {winCondition} | Type: {boardType}");
    }

    public static void ShowError(string message)
    {
        Console.WriteLine();
        Console.WriteLine($"Error: {message}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    
    public static void ShowNextPlayer(bool isNextPlayerX)
    {
        Console.WriteLine("Next Player: " + (isNextPlayerX ? "🔴 Red" : "🔵 Blue"));
    }

    private static string GetCellRepresentation(ECellState cellValue)
    {
        return cellValue switch
        {
            ECellState.Empty => "⚪️ ",
            ECellState.Red => "🔴 ",
            ECellState.Blue => "🔵 ",
            ECellState.RedWin => "❌ ",
            ECellState.BlueWin => "🔵 ",
            _ => " ? "
        };
    }
}