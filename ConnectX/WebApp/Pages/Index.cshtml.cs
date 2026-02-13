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

                var config = GameConfiguration.FromGameState(gameState);
                var gameBrain = new GameBrain(config, gameState.Player1Name, gameState.Player2Name);
                gameBrain.LoadFromGameState(gameState);

                SavedGames.Add(new SavedGameInfo
                {
                    Id = id,
                    Description = description,
                    IsFinished = gameBrain.IsGameFinished()
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

    public class SavedGameInfo
    {
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsFinished { get; set; }
    }
}
