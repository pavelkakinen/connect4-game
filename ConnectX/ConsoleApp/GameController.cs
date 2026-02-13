using BLL;
using BLL.AI;
using ConsoleUI;
using Domain;
using DAL;
using MenuSystem;

namespace ConsoleApp;

public class GameController
{
    private const int AiDepth = 6;

    private GameBrain GameBrain { get; set; }
    private IRepository<GameState> _gameRepository;
    private IAIPlayer? _aiPlayer1;
    private IAIPlayer? _aiPlayer2;
    private string _player1Name;
    private string _player2Name;
    private string? _currentGameId;

    public GameController(
        GameConfiguration configuration,
        string player1Name,
        string player2Name,
        IRepository<GameState> gameRepository)
    {
        GameBrain = new GameBrain(configuration, player1Name, player2Name);
        _gameRepository = gameRepository;
        _player1Name = player1Name;
        _player2Name = player2Name;
        _currentGameId = null;

        InitializeAI(configuration);
    }

    private void InitializeAI(GameConfiguration config)
    {
        _aiPlayer1 = config.P1Type == EPlayerType.Computer
            ? new MinimaxAI(maxDepth: AiDepth)
            : null;

        _aiPlayer2 = config.P2Type == EPlayerType.Computer
            ? new MinimaxAI(maxDepth: AiDepth)
            : null;
    }

    public string GameLoop()
    {
        while (true)
        {
            if (IsGameAlreadyOver())
                return ShowFinishedGame();

            var (column, isAI) = GetNextMove();

            if (column == -1)
            {
                var pauseAction = ShowPauseMenu();
                if (pauseAction == "continue") continue;
                if (pauseAction == "m") return "m";
            }

            var result = GameBrain.ProcessMove(column);

            if (!result.success)
            {
                Console.WriteLine("The column is full. Press any key to continue.");
                Console.ReadKey();
                continue;
            }

            if (isAI)
                ShowAIMove(column);

            if (CheckAndHandleWin(result.row, column))
                return "m";

            if (CheckAndHandleDraw())
                return "m";
        }
    }

    private bool IsGameAlreadyOver()
    {
        var (currentWinner, _) = GameBrain.CheckWin();
        return currentWinner != ECellState.Empty || GameBrain.BoardIsFull();
    }

    private string ShowFinishedGame()
    {
        Console.Clear();

        var (currentWinner, currentWinningCells) = GameBrain.CheckWin();
        if (currentWinner != ECellState.Empty)
        {
            Ui.DrawWinningBoard(GameBrain.GetBoard(), currentWinner, currentWinningCells);
            var winnerName = currentWinner == ECellState.Red ? _player1Name : _player2Name;
            Console.WriteLine($"\n🎉 {winnerName} WINS! 🎉");
        }
        else
        {
            Ui.DrawBoard(GameBrain.GetBoard(), 0);
            Console.WriteLine("\n DRAW! Board is full! ");
        }

        Console.WriteLine("\n============================");
        Console.WriteLine("   GAME ALREADY FINISHED     ");
        Console.WriteLine("============================");
        Console.WriteLine("\nThis game has ended. You can only view it.");
        Console.WriteLine("Press any key to return to menu...");
        Console.ReadKey();
        return "m";
    }

    private (int column, bool isAI) GetNextMove()
    {
        bool isRedTurn = GameBrain.NextMoveByRed;

        if (isRedTurn && _aiPlayer1 != null)
            return (GetAIMove(_aiPlayer1, isRedPlayer: true), true);

        if (!isRedTurn && _aiPlayer2 != null)
            return (GetAIMove(_aiPlayer2, isRedPlayer: false), true);

        return (GetColumnChoice(), false);
    }

    private void ShowAIMove(int column)
    {
        Console.Clear();
        Ui.DrawBoard(GameBrain.GetBoard(), 0);
        Console.WriteLine($"\n AI played column {column + 1}");
        Thread.Sleep(1500);
    }

    private bool CheckAndHandleWin(int row, int column)
    {
        var winner = GameBrain.CheckWin(row, column);
        if (winner.winner == ECellState.Empty) return false;

        Console.Clear();
        Ui.DrawWinningBoard(GameBrain.GetBoard(), winner.winner, winner.winningCells);

        var winnerName = winner.winner == ECellState.Red ? _player1Name : _player2Name;
        Ui.PrintGameResult(winner.winner, winnerName);

        SaveCurrentGame();

        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
        return true;
    }

    private bool CheckAndHandleDraw()
    {
        if (!GameBrain.BoardIsFull()) return false;

        Console.Clear();
        Ui.DrawBoard(GameBrain.GetBoard(), 0);
        Console.WriteLine("\n DRAW! Board is full! ");

        SaveCurrentGame();

        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
        return true;
    }

    private void SaveCurrentGame()
    {
        try
        {
            var gameState = GameBrain.GetGameState(_currentGameId);

            var gameId = _gameRepository.Save(gameState);
            _currentGameId = gameId;

            Console.WriteLine("\n=============================");
            Console.WriteLine("    GAME SAVED TO HISTORY     ");
            Console.WriteLine("=============================");
            Console.WriteLine($"Game ID: {gameId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n️  Could not save game: {ex.Message}");
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

    private string ShowPauseMenu()
    {
        var pauseMenu = new Menu("GAME PAUSED", EMenuLevel.Pause);

        pauseMenu.AddMenuItem("1", "Continue Game", () => "continue");
        pauseMenu.AddMenuItem("2", "Save and Exit", () =>
        {
            SaveCurrentGame();
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

            _player1Name = gameState.Player1Name;
            _player2Name = gameState.Player2Name;

            InitializeAI(GameBrain.GetConfiguration());

            _currentGameId = gameId;

            ShowLoadedGameMessage(gameState);
        }
        catch (FileNotFoundException ex)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       ERROR!                 ");
            Console.WriteLine("==============================");
            Console.WriteLine(ex.Message);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private void ShowLoadedGameMessage(GameState gameState)
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("       GAME LOADED!           ");
        Console.WriteLine("==============================");
        Console.WriteLine($"Loaded: {gameState.Player1Name} vs {gameState.Player2Name}");
        Console.WriteLine($"Saved at: {gameState.SavedAt:yyyy-MM-dd HH:mm}");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}
