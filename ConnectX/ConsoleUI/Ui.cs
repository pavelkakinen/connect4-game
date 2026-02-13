using Domain;

namespace ConsoleUI;

public static class Ui
{
    public static void DrawBoard(ECellState[,] gameBoard, int selectedColumn)
    {
        Console.Clear();
        for (int i = 0; i < gameBoard.GetLength(1); i++)
        {
            if (i == selectedColumn)
            {
                Console.Write("⬇️ ");
            }
            else
            {
                Console.Write("   ");
            }
        }
        Console.WriteLine();
        for (int row = 0; row < gameBoard.GetLength(0); row++)
        {
            for (int col = 0; col < gameBoard.GetLength(1); col++)
            {
                Console.Write(GetCellRepresentation(gameBoard[row, col]));
            }
            Console.WriteLine();
        }

    }

    public static void DrawWinningBoard(
        ECellState[,] gameBoard,
        ECellState winner,
        List<(int row, int col)> winningCells
        )
    {
        for (int row = 0; row < gameBoard.GetLength(0); row++)
        {
            for (int col = 0; col < gameBoard.GetLength(1); col++)
            {
                if (winningCells.Contains((row, col)))
                {
                    var winCell = winner == ECellState.Red ? ECellState.RedWin : ECellState.BlueWin;
                    Console.Write(GetCellRepresentation(winCell));
                }
                else
                {
                    Console.Write(GetCellRepresentation(gameBoard[row, col]));
                }
            }
            Console.WriteLine();
        }
    }

    public static void PrintGameResult(ECellState winner, string winnerName)
    {
        var (emoji, label) = winner switch
        {
            ECellState.Red => ("🔴", $"{winnerName} WINS"),
            ECellState.Blue => ("🔵", $"{winnerName} WINS"),
            _ => ("", "DRAW GAME")
        };

        Console.WriteLine("====================");
        Console.WriteLine($"    {label} {emoji}");
        Console.WriteLine("====================");
    }

    private static string GetCellRepresentation(ECellState cellValue) =>
        cellValue switch
        {
            ECellState.Empty => "⬜️ ",
            ECellState.Red => "🔴 ",
            ECellState.Blue => "🔵 ",
            ECellState.RedWin => "❌  ",
            ECellState.BlueWin => "💙 ",
            _ => " ? "
        };

    public static (string player1Name, string player2Name) GetPlayerNames(bool isVsComputer)
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("     ENTER PLAYER NAMES       ");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.Write("Player 1 (Red 🔴) name: ");
        var player1Name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(player1Name))
        {
            player1Name = "Player 1";
        }

        string player2Name;
        if (isVsComputer)
        {
            player2Name = "Computer";
        }
        else
        {
            Console.Write("Player 2 (Blue 🔵) name: ");
            player2Name = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(player2Name))
            {
                player2Name = "Player 2";
            }
        }

        return (player1Name, player2Name);
    }
}
