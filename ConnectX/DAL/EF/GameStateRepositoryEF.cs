using Microsoft.EntityFrameworkCore;

namespace DAL.EF;

public class GameStateRepositoryEF : IRepository<GameState>
{
    private readonly AppDbContext _dbContext;
    
    public GameStateRepositoryEF(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public List<(string id, string description)> List()
    {
        var res = new List<(string id, string description)>();
        
        foreach (var game in _dbContext.Games.OrderByDescending(g => g.SavedAt))
        {
            res.Add((
                game.GameId,
                $"{game.Player1Name} vs {game.Player2Name} - {game.SavedAt:yyyy-MM-dd HH:mm}"
            ));
        }
        
        return res;
    }
    
    public async Task<List<(string id, string description)>> ListAsync()
    {
        var res = new List<(string id, string description)>();
        
        foreach (var game in await _dbContext.Games.OrderByDescending(g => g.SavedAt).ToListAsync())
        {
            res.Add((
                game.GameId,
                $"{game.Player1Name} vs {game.Player2Name} - {game.SavedAt:yyyy-MM-dd HH:mm}"
            ));
        }
        
        return res;
    }
    
    public string Save(GameState data)
    {
        // if ID ie empty, make the new
        if (string.IsNullOrEmpty(data.GameId))
        {
            data.GameId = GenerateGameId();
        }
        
        data.SavedAt = DateTime.Now;
        
        // if exists
        var existingGame = _dbContext.Games.Find(data.GameId);
        
        if (existingGame != null)
        {
            // Update existing
            _dbContext.Entry(existingGame).CurrentValues.SetValues(data);
            existingGame.Board = data.Board;
        }
        else
        {
            // Add new
            _dbContext.Games.Add(data);
        }
        
        _dbContext.SaveChanges();
        return data.GameId;
    }
    
    public GameState Load(string id)
    {
        return _dbContext.Games.Find(id)!;
    }
    
    public void Delete(string id)
    {
        var game = _dbContext.Games.Find(id);
        if (game != null)
        {
            _dbContext.Games.Remove(game);
            _dbContext.SaveChanges();
        }
    }
    
    private static string GenerateGameId()
    {
        return $"game_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString()[..8]}";
    }
}