using BLL;

namespace ConsoleUI;

public static class Ui
{
    public static void DrawBoard(ECellState[,] gameBoard)
    {
        Console.Clear();
        
        int height = gameBoard.GetLength(0);
        int width = gameBoard.GetLength(1);
        
        for (int col = 0; col < width; col++)
        {
            if (col == 0)
            {
                Console.Write(GetNumberRepresentation(col + 1));
            }
            else
            {
                Console.Write("|" + GetNumberRepresentation(col + 1));
            }
        }
        Console.WriteLine();
        
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width - 1; col++)
            {
                Console.Write("---+");
            }
            Console.Write("---");
            Console.WriteLine();

            for (int col = 0; col < width; col++)
            {
                if (col == 0)
                {
                    Console.Write(GetCellRepresentation(gameBoard[row, col]));
                }
                else
                {
                    Console.Write("|" + GetCellRepresentation(gameBoard[row, col]));
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public static void ShowWinner(ECellState winner)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        if (winner == ECellState.XWin)
        {
            Console.WriteLine("       PLAYER O WINS!");
        }
        else if (winner == ECellState.OWin)
        {
            Console.WriteLine("       PLAYER X WINS!");
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
        Console.WriteLine();
    }

    public static void ShowError(string message)
    {
        Console.WriteLine();
        Console.WriteLine($"Error: {message}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static string GetNumberRepresentation(int number)
    {
        return " " + (number < 10 ? "0" + number : number.ToString());
    }
    
    public static void ShowNextPlayer(bool isNextPlayerX)
    {
        Console.WriteLine("Next Player: " + (isNextPlayerX ? "X" : "O"));
    }

    private static string GetCellRepresentation(ECellState cellValue)
    {
        return cellValue switch
        {
            ECellState.Empty => "   ",
            ECellState.X => " X ",
            ECellState.O => " O ",
            ECellState.XWin => "XXX",
            ECellState.OWin => "OOO",
            _ => " ? "
        };
    }
}