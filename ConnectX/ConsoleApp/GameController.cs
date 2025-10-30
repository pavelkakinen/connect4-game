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
        var GameConfig = new GameConfiguration();
        
        bool WidthIsSet = false;
        while (!WidthIsSet)
        {
            Console.Clear();
            Console.WriteLine("Set Game Board Width: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var width))
            {
                GameConfig.setWidth(width);
                WidthIsSet = true;
            }
        }
        
        bool HeightIsSet = false;
        while (!HeightIsSet)
        {
            Console.Clear();
            Console.WriteLine("Set Game Board Height: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var height))
            {
                GameConfig.setHeight(height);
                HeightIsSet = true;
            }
        }
        
        bool WinCondSet = false;
        while (!WinCondSet)
        {
            Console.Clear();
            Console.WriteLine("Set Game Sin Condition: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var cond))
            {
                GameConfig.setWinCondition(cond);
                WinCondSet = true;
            }
        }
        
        bool PlayerTypesSet = false;
        while (!PlayerTypesSet)
        {
            Console.Clear();
            Console.WriteLine("Set Game Player Types: ");
            Console.WriteLine("1) Human to Human");
            Console.WriteLine("2) Human to Computer");
            Console.WriteLine("3) Computer to Computer");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out var x))
            {
                if (x == 1)
                {
                    GameConfig.setP1Type(EPlayerType.Human);
                    GameConfig.setP2Type(EPlayerType.Human);
                    PlayerTypesSet = true;
                } else if (x == 2)
                {
                    GameConfig.setP1Type(EPlayerType.Human);
                    GameConfig.setP2Type(EPlayerType.Computer);
                    PlayerTypesSet = true;
                }  else if (x == 3)
                {
                    GameConfig.setP1Type(EPlayerType.Computer);
                    GameConfig.setP2Type(EPlayerType.Computer);
                    PlayerTypesSet = true;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input");
                    Console.WriteLine("Press any key to continue..");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input");
                Console.WriteLine("Press any key to continue..");
            }
        }
        return  GameConfig;
    }

    public void GameLoop()
    {
        var GameOver = false;
        do
        {
            Console.Clear();

            Ui.DrawBoard(GameBrain.GetBoard());
            Ui.ShowNextPlayer(GameBrain.isNextPlayerX());
            
            Console.Write("Choice: ");
            var input = Console.ReadLine();
            if (input?.ToLower() == "x")
            {
                GameOver = true;
            }

            if (int.TryParse(input, out var x))
            {
                bool moveSuccessful = GameBrain.ProcessMove(x - 1);

                if (!moveSuccessful)
                {
                    Console.WriteLine("Invalid input. Column is full or choice is out of range.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            
            var winner = GameBrain.GetWinner(x - 1);
            if (winner != ECellState.Empty)
            {
                // TODO: move to ui
                Console.WriteLine("Winner is: " + (winner == ECellState.XWin ? "X" : "O"));
                break;
            }
        } while (GameOver == false);

    }
}