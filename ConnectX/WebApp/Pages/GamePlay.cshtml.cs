using BLL;
using BLL.AI;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class GamePlay : PageModel
{
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
    {
        try
        {
            var savedState = _gameStateRepo.Load(gameId);
            
            GameConfiguration = new GameConfiguration
            {
                BoardWidth = savedState.BoardWidth,
                BoardHeight = savedState.BoardHeight,
                WinCondition = savedState.WinCond,
                BoardType = savedState.BoardType
            };
            
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
            //  invalid
        }
    }
    
    // Create new game if we have configuration info
    if (boardWidth.HasValue && boardHeight.HasValue && winCondition.HasValue &&
        !string.IsNullOrEmpty(player1Name) && !string.IsNullOrEmpty(player2Name))
    {
        GameConfiguration = new GameConfiguration
        {
            BoardWidth = boardWidth.Value,
            BoardHeight = boardHeight.Value,
            WinCondition = winCondition.Value,
            BoardType = (EBoardType)(boardType ?? 0)
        };
        
        GameConfiguration.SetP1Type((EPlayerType)(p1Type ?? 0));
        GameConfiguration.SetP2Type((EPlayerType)(p2Type ?? 0));
        
        GameBrain = new GameBrain(GameConfiguration, player1Name, player2Name);
        
        InitializeAI();
        ProcessAIMoves();
        
        var initialState = GameBrain.GetGameState();
        initialState.P1Type = (int)GameConfiguration.P1Type;
        initialState.P2Type = (int)GameConfiguration.P2Type;
        
        GameId = _gameStateRepo.Save(initialState);
        
        PopulateViewProperties();
        return Page();
    }
    
    return RedirectToPage("./NewGame");
}
    
    private void CheckForWinner()
    {
        var board = GameBrain.GetBoard();
        
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                var cellState = board[row, col];
                
                if (cellState != ECellState.Empty)
                {
                    var winCheck = GameBrain.CheckWin(row, col);
                    
                    if (winCheck.winner != ECellState.Empty)
                    {
                        Winner = winCheck.winner;
                        WinningCells = winCheck.winningCells;
                        return;
                    }
                }
            }
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
        if (GameConfiguration.P1Type == EPlayerType.Computer)
        {
            _aiPlayer1 = new MinimaxAI(maxDepth: 6);
        }
        
        if (GameConfiguration.P2Type == EPlayerType.Computer)
        {
            _aiPlayer2 = new MinimaxAI(maxDepth: 6);
        }
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
        var state = GameBrain.GetGameState();
        state.GameId = GameId;
        state.P1Type = (int)GameConfiguration.P1Type;
        state.P2Type = (int)GameConfiguration.P2Type;
    
        Console.WriteLine($"BEFORE SAVE: {state.GameId} at {state.SavedAt:HH:mm:ss.fff}");
    
        _gameStateRepo.Save(state);
    
        Console.WriteLine($"AFTER SAVE: {state.GameId}");
    }

    public IActionResult OnGetCheckUpdate(string gameId, long lastUpdateTimestamp)
    {
        try
        {
            var currentState = _gameStateRepo.Load(gameId);
            var serverTimestamp = new DateTimeOffset(currentState.SavedAt).ToUnixTimeSeconds();
            bool hasUpdate = serverTimestamp > lastUpdateTimestamp;
        
            Console.WriteLine($"CHECK: client={lastUpdateTimestamp}, server={serverTimestamp}, updated={hasUpdate}");
        
            return new JsonResult(new
            {
                updated = hasUpdate,
                timestamp = serverTimestamp
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CHECK ERROR: {ex.Message}");
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