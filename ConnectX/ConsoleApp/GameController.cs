using BLL;
using BLL.AI;
using ConsoleUI;
using Domain;
using DAL;
using MenuSystem;

namespace ConsoleApp;

public class GameController
{
    private GameBrain GameBrain {get; set;}
    private IRepository<GameState> _gameRepository;
    private IAIPlayer? _aiPlayer1;  // ← НОВОЕ! AI для игрока 1
    private IAIPlayer? _aiPlayer2;  // ← НОВОЕ! AI для игрока 2

    public GameController(
        GameConfiguration configuration, 
        string player1Name, 
        string player2Name,
        IRepository<GameState> gameRepository)
    {
        GameBrain = new GameBrain(configuration, player1Name, player2Name);
        _gameRepository = gameRepository;
        if (configuration.P1Type == EPlayerType.Computer)
        {
            _aiPlayer1 = new MinimaxAI(maxDepth: 6);  // AI для игрока 1
        }
        
        if (configuration.P2Type == EPlayerType.Computer)
        {
            _aiPlayer2 = new MinimaxAI(maxDepth: 6);  // AI для игрока 2
        }
    }

    public string GameLoop()
    {
        while (true)
        {
            int columnChoice;
            bool isRedTurn = GameBrain.NextMoveByRed;
            bool isAIMove = false;
            
            if (isRedTurn && _aiPlayer1 != null)
            {
                // AI Player 1
                columnChoice = GetAIMove(_aiPlayer1, isRedPlayer: true);
                isAIMove = true;
            }
            else if (!isRedTurn && _aiPlayer2 != null)
            {
                // AI Player 2
                columnChoice = GetAIMove(_aiPlayer2, isRedPlayer: false);
                isAIMove = true;
            }
            else
            {
                // Human player
                columnChoice = GetColumnChoice();
                
                if (columnChoice == -1)
                {
                    var pauseAction = ShowPauseMenu();
                    if (pauseAction == "continue") continue;
                    else if (pauseAction == "m") return "m";
                }
            }
            
            // Сделай ход
            var result = GameBrain.ProcessMove(columnChoice);
            
            if (!result.success)
            {
                Console.WriteLine("The column is full. Press any key to continue.");
                Console.ReadKey();
                continue;
            }
            
            // Если AI ходил - покажи что он сделал
            if (isAIMove)
            {
                Console.Clear();
                Ui.DrawBoard(GameBrain.GetBoard(), 0);
                Console.WriteLine($"\n🤖 AI played column {columnChoice + 1}");
                Console.ReadKey();
            }
            
            // Проверка победы
            var winner = GameBrain.CheckWin(result.row, columnChoice);
            if (winner.winner != ECellState.Empty)
            {
                Console.Clear();
                Ui.DrawWinningBoard(GameBrain.GetBoard(), winner.winner, winner.winningCells);
                Ui.PrintGameResult(winner.winner);
                SaveCompletedGame(winner.winner);
                return "m";
            }

            if (GameBrain.BoardIsFull())
            {
                Ui.DrawBoard(GameBrain.GetBoard(), 0);
                Ui.PrintGameResult(ECellState.Empty);
                SaveCompletedGame(ECellState.Empty);
                return "m";
            }
        }
    }

    private void SaveCompletedGame(ECellState winner)
    {
        try
        {
            var gameState = GameBrain.GetGameState();
            _gameRepository.Save(gameState);
        }
        catch
        {
        }
    }

    private int GetAIMove(IAIPlayer aiPlayer, bool isRedPlayer)
    {
        var board = GameBrain.GetBoard();
        var config = GameBrain.GetConfiguration();
        
        return aiPlayer.GetBestMove(board, isRedPlayer, config);
    }

    private int GetColumnChoice()
    {
        var gameBoard = GameBrain.GetBoard();
        int selectedColumn = 0;
        
        while (true)
        {
            Ui.DrawBoard(gameBoard, selectedColumn);
            
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    selectedColumn = selectedColumn == 0 ? gameBoard.GetLength(1) - 1 : selectedColumn - 1;
                    break;
                case ConsoleKey.RightArrow:
                    selectedColumn = selectedColumn == gameBoard.GetLength(1) - 1 ? 0 : selectedColumn + 1;
                    break;
                case ConsoleKey.Enter:
                    return selectedColumn;
                case ConsoleKey.Q:
                    return -1;
            }
        }
    }
    
    public string SaveGame()
    {
        var gameState = GameBrain.GetGameState();
        var gameId = _gameRepository.Save(gameState);
    
        Console.Clear();
        Console.WriteLine("╔════════════════════════════╗");
        Console.WriteLine("║      GAME SAVED!           ║");
        Console.WriteLine("╚════════════════════════════╝");
        Console.WriteLine($"Game ID: {gameId}");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    
        return gameId;
    }

    private string ShowPauseMenu()
    {
        var pauseMenu = new Menu("GAME PAUSED", EMenuLevel.Pause);
        
        pauseMenu.AddMenuItem("1", "Continue Game", () => "continue");
        pauseMenu.AddMenuItem("2", "Save and Exit", () =>
        {
            SaveGame();
            return "m";
        });
        pauseMenu.AddMenuItem("3", "Exit without Saving", () => "m");
        
        var result = pauseMenu.Run();
        
        return result;

    }
    
    public void LoadGame(string gameId)
    {
        try
        {
            var gameState = _gameRepository.Load(gameId);
            GameBrain.LoadFromGameState(gameState);
        
            Console.Clear();
            Console.WriteLine("╔════════════════════════════╗");
            Console.WriteLine("║      GAME LOADED!          ║");
            Console.WriteLine("╚════════════════════════════╝");
            Console.WriteLine($"Loaded: {gameState.Player1Name} vs {gameState.Player2Name}");
            Console.WriteLine($"Saved at: {gameState.SavedAt:yyyy-MM-dd HH:mm}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        catch (FileNotFoundException ex)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════╗");
            Console.WriteLine("║      ERROR!                ║");
            Console.WriteLine("╚════════════════════════════╝");
            Console.WriteLine(ex.Message);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
