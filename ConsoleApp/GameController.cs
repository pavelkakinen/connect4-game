using BLL;
using BLL.AI;
using ConsoleUI;
using Domain;
using DAL;
using MenuSystem;

namespace ConsoleApp;

public class GameController
{
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
        
        if (configuration.P1Type == EPlayerType.Computer)
        {
            _aiPlayer1 = new MinimaxAI(maxDepth: 6);
        }

        if (configuration.P2Type == EPlayerType.Computer)
        {
            _aiPlayer2 = new MinimaxAI(maxDepth: 6);
        }
    }

    public string GameLoop()
    {
        while (true)
        {
            // check if game already over
            var (currentWinner, currentWinningCells) = GameBrain.CheckWin();
            if (currentWinner != ECellState.Empty || GameBrain.BoardIsFull())
            {
                // Game is already finished - show final state and exit
                Console.Clear();
                
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
                    if (pauseAction == "m") return "m";
                }
            }

            // Make move
            var result = GameBrain.ProcessMove(columnChoice);

            if (!result.success)
            {
                Console.WriteLine("The column is full. Press any key to continue.");
                Console.ReadKey();
                continue;
            }

            // If AI moved - show what it did
            if (isAIMove)
            {
                Console.Clear();
                Ui.DrawBoard(GameBrain.GetBoard(), 0);
                Console.WriteLine($"\n AI played column {columnChoice + 1}");
                Thread.Sleep(1500);
            }

            // Check for win
            var winner = GameBrain.CheckWin(result.row, columnChoice);
            if (winner.winner != ECellState.Empty)
            {
                Console.Clear();
                Ui.DrawWinningBoard(GameBrain.GetBoard(), winner.winner, winner.winningCells);
                
                var winnerName = winner.winner == ECellState.Red ? _player1Name : _player2Name;
                Ui.PrintGameResult(winner.winner, winnerName);
                
                SaveCompletedGame(winner.winner);
                
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                return "m";
            }

            // Check for draw
            if (GameBrain.BoardIsFull())
            {
                Console.Clear();
                Ui.DrawBoard(GameBrain.GetBoard(), 0);
                Console.WriteLine("\n DRAW! Board is full! ");
                
                SaveCompletedGame(ECellState.Empty);
                
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                return "m";
            }
        }
    }

    private void SaveCompletedGame(ECellState winner)
    {
        try
        {
            var gameState = GameBrain.GetGameState();
            
            if (!string.IsNullOrEmpty(_currentGameId))
            {
                gameState.GameId = _currentGameId;
            }
            
            var gameId = _gameRepository.Save(gameState);
            
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

    private string SaveGame()
    {
        var gameState = GameBrain.GetGameState();
        
        if (!string.IsNullOrEmpty(_currentGameId))
        {
            gameState.GameId = _currentGameId;
        }
        
        var gameId = _gameRepository.Save(gameState);

        _currentGameId = gameId;
        
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("       GAME SAVED!            ");
        Console.WriteLine("==============================");
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
            
            // Update player names from loaded state
            _player1Name = gameState.Player1Name;
            _player2Name = gameState.Player2Name;
            
            _currentGameId = gameId;

            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       GAME LOADED!           ");
            Console.WriteLine("==============================");
            Console.WriteLine($"Loaded: {gameState.Player1Name} vs {gameState.Player2Name}");
            Console.WriteLine($"Saved at: {gameState.SavedAt:yyyy-MM-dd HH:mm}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
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
}