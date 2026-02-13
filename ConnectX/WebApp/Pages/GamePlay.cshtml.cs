using BLL;
using BLL.AI;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class GamePlay : PageModel
{
    private const int AiDepth = 6;

    private readonly IRepository<GameState> _gameStateRepo;

    public GamePlay(IRepository<GameState> gameStateRepo)
    {
        _gameStateRepo = gameStateRepo;
    }

    public string GameId { get; set; } = default!;
    public GameBrain GameBrain { get; set; } = default!;
    public GameConfiguration GameConfiguration { get; set; } = default!;

    public bool IsGameOver { get; private set; }
    public string CurrentPlayerName { get; private set; } = default!;
    public ECellState? Winner { get; private set; }
    public string? WinnerName { get; private set; }
    public List<(int row, int col)> WinningCells { get; private set; } = new();
    public bool ShowPauseMenu { get; set; }

    private IAIPlayer? _aiPlayer1;
    private IAIPlayer? _aiPlayer2;

    public IActionResult OnGet(
        string? gameId,
        int? boardWidth,
        int? boardHeight,
        int? winCondition,
        int? boardType,
        string? player1Name,
        string? player2Name,
        int? p1Type,
        int? p2Type,
        int? col,
        bool pause = false)
    {
        ShowPauseMenu = pause;

        if (!string.IsNullOrEmpty(gameId))
            return LoadExistingGame(gameId, col, pause);

        if (boardWidth.HasValue && boardHeight.HasValue && winCondition.HasValue &&
            !string.IsNullOrEmpty(player1Name) && !string.IsNullOrEmpty(player2Name))
            return CreateNewGame(boardWidth.Value, boardHeight.Value, winCondition.Value,
                boardType ?? 0, player1Name, player2Name, p1Type ?? 0, p2Type ?? 0);

        return RedirectToPage("./NewGame");
    }

    private IActionResult LoadExistingGame(string gameId, int? col, bool pause)
    {
        try
        {
            var savedState = _gameStateRepo.Load(gameId);

            GameConfiguration = GameConfiguration.FromGameState(savedState);
            GameConfiguration.SetP1Type((EPlayerType)savedState.P1Type);
            GameConfiguration.SetP2Type((EPlayerType)savedState.P2Type);

            GameBrain = new GameBrain(GameConfiguration, savedState.Player1Name, savedState.Player2Name);
            GameBrain.LoadFromGameState(savedState);

            GameId = gameId;

            InitializeAI();
            CheckForWinner();

            if (col.HasValue && !IsGameOver && !pause)
            {
                ProcessPlayerMove(col.Value);
                ProcessAIMoves();
                SaveGameState();
            }

            PopulateViewProperties();
            return Page();
        }
        catch (Exception)
        {
            return RedirectToPage("./NewGame");
        }
    }

    private IActionResult CreateNewGame(int width, int height, int winCond,
        int boardType, string p1Name, string p2Name, int p1Type, int p2Type)
    {
        GameConfiguration = new GameConfiguration
        {
            BoardWidth = width,
            BoardHeight = height,
            WinCondition = winCond,
            BoardType = (EBoardType)boardType
        };

        GameConfiguration.SetP1Type((EPlayerType)p1Type);
        GameConfiguration.SetP2Type((EPlayerType)p2Type);

        GameBrain = new GameBrain(GameConfiguration, p1Name, p2Name);

        InitializeAI();
        ProcessAIMoves();

        var initialState = GameBrain.GetGameState();
        initialState.P1Type = (int)GameConfiguration.P1Type;
        initialState.P2Type = (int)GameConfiguration.P2Type;

        GameId = _gameStateRepo.Save(initialState);

        PopulateViewProperties();
        return Page();
    }

    private void CheckForWinner()
    {
        var result = GameBrain.CheckWin();
        if (result.winner != ECellState.Empty)
        {
            Winner = result.winner;
            WinningCells = result.winningCells;
        }
    }

    public IActionResult OnPostSaveAndExit(string gameId)
    {
        return RedirectToPage("./Index");
    }

    public IActionResult OnPostDeleteAndExit(string gameId)
    {
        try
        {
            _gameStateRepo.Delete(gameId);
        }
        catch
        {
        }

        return RedirectToPage("./Index");
    }

    public IActionResult OnPostContinue(string gameId)
    {
        return RedirectToPage("./GamePlay", new { gameId = gameId });
    }

    private void InitializeAI()
    {
        _aiPlayer1 = GameConfiguration.P1Type == EPlayerType.Computer
            ? new MinimaxAI(maxDepth: AiDepth)
            : null;

        _aiPlayer2 = GameConfiguration.P2Type == EPlayerType.Computer
            ? new MinimaxAI(maxDepth: AiDepth)
            : null;
    }

    private void ProcessPlayerMove(int col)
    {
        if (col < 0 || col >= GameConfiguration.BoardWidth)
            return;

        var result = GameBrain.ProcessMove(col);
        if (result.success)
        {
            var winCheck = GameBrain.CheckWin(result.row, col);
            if (winCheck.winner != ECellState.Empty)
            {
                Winner = winCheck.winner;
                WinningCells = winCheck.winningCells;
            }
        }
    }

    private void ProcessAIMoves()
    {
        while (!IsGameOverCheck())
        {
            bool isRedTurn = GameBrain.NextMoveByRed;
            IAIPlayer? currentAI = isRedTurn ? _aiPlayer1 : _aiPlayer2;

            if (currentAI == null)
                break;

            var aiMove = currentAI.GetBestMove(GameBrain.GetBoard(), isRedTurn, GameConfiguration);
            var result = GameBrain.ProcessMove(aiMove);

            if (result.success)
            {
                var winCheck = GameBrain.CheckWin(result.row, aiMove);
                if (winCheck.winner != ECellState.Empty)
                {
                    Winner = winCheck.winner;
                    WinningCells = winCheck.winningCells;
                    break;
                }
            }

            if (GameBrain.BoardIsFull())
                break;
        }
    }

    private bool IsGameOverCheck()
    {
        return Winner != null || GameBrain.BoardIsFull();
    }

    private void SaveGameState()
    {
        var state = GameBrain.GetGameState(GameId);
        state.P1Type = (int)GameConfiguration.P1Type;
        state.P2Type = (int)GameConfiguration.P2Type;

        _gameStateRepo.Save(state);
    }

    public IActionResult OnGetCheckUpdate(string gameId, long lastUpdateTimestamp)
    {
        try
        {
            var currentState = _gameStateRepo.Load(gameId);
            var serverTimestamp = new DateTimeOffset(currentState.SavedAt).ToUnixTimeSeconds();
            bool hasUpdate = serverTimestamp > lastUpdateTimestamp;

            return new JsonResult(new
            {
                updated = hasUpdate,
                timestamp = serverTimestamp
            });
        }
        catch (Exception)
        {
            return new JsonResult(new { updated = false });
        }
    }

    private void PopulateViewProperties()
    {
        var state = GameBrain.GetGameState();

        IsGameOver = IsGameOverCheck();
        CurrentPlayerName = state.IsNextMoveByRed ? state.Player1Name : state.Player2Name;

        if (Winner == ECellState.Red)
        {
            WinnerName = state.Player1Name;
        }
        else if (Winner == ECellState.Blue)
        {
            WinnerName = state.Player2Name;
        }
    }
}
