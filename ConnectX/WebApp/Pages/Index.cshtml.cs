using BLL;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IRepository<GameState> _gameStateRepo;
    
    public IndexModel(IRepository<GameState> gameStateRepo)
    {
        _gameStateRepo = gameStateRepo;
    }
    
    public List<SavedGameInfo> SavedGames { get; set; } = default!;
    
    public async Task OnGetAsync()
    {
        var games = await _gameStateRepo.ListAsync();
        SavedGames = new List<SavedGameInfo>();
        
        foreach (var (id, description) in games)
        {
            try
            {
                var gameState = _gameStateRepo.Load(id);
                
                // Check if game is finished
                var isFinished = CheckIfGameFinished(gameState);
                
                SavedGames.Add(new SavedGameInfo
                {
                    Id = id,
                    Description = description,
                    IsFinished = isFinished
                });
            }
            catch
            {
                // Skip invalid games
            }
        }
    }
    
    public async Task<IActionResult> OnPostDeleteAsync(string gameId)
    {
        try
        {
            _gameStateRepo.Delete(gameId);
        }
        catch
        {
            // Ignore errors
        }
        
        return RedirectToPage();
    }
    
    private bool CheckIfGameFinished(GameState gameState)
    {
        // Recreate GameBrain to check game state
        var config = new GameConfiguration
        {
            BoardWidth = gameState.BoardWidth,
            BoardHeight = gameState.BoardHeight,
            WinCondition = gameState.WinCond,
            BoardType = gameState.BoardType
        };
        
        var gameBrain = new GameBrain(config, gameState.Player1Name, gameState.Player2Name);
        gameBrain.LoadFromGameState(gameState);
        
        if (gameBrain.BoardIsFull())
            return true;
        
        // Check if there's a winner by checking all cells
        var board = gameBrain.GetBoard();
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] != ECellState.Empty)
                {
                    var winCheck = gameBrain.CheckWin(row, col);
                    if (winCheck.winner != ECellState.Empty)
                    {
                        return true; // Game has a winner
                    }
                }
            }
        }
        
        return false;
    }
    
    public class SavedGameInfo
    {
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsFinished { get; set; }
    }
}