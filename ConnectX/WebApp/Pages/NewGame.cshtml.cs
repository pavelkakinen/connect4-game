using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class NewGame : PageModel
{
    public SelectList ConfigurationSelectList { get; set; } = default!;
    
    [BindProperty]
    [Required]
    public string ConfigId { get; set; } = default!;
    
    [BindProperty]
    [Required]
    [Length(1, 32)]
    public string Player1Name { get; set; } = "Player 1";
    
    [BindProperty]
    [Required]
    [Length(1, 32)]
    public string Player2Name { get; set; } = "Player 2";
    
    [BindProperty]
    public int P1Type { get; set; } = 0;  // 0 = Human, 1 = AI
    
    [BindProperty]
    public int P2Type { get; set; } = 0;  // 0 = Human, 1 = AI
    
    public void OnGet()
    {
        LoadConfigurations();
    }
    
    private void LoadConfigurations()
    {
        var configurations = new[]
        {
            new { Id = "classic", Name = "Classic Connect 4 (7×6, Win 4)", Width = 7, Height = 6, Win = 4, Type = 0 },
            new { Id = "connect3", Name = "Connect 3 (5×4, Win 3)", Width = 5, Height = 4, Win = 3, Type = 0 },
            new { Id = "connect5", Name = "Connect 5 (9×7, Win 5)", Width = 9, Height = 7, Win = 5, Type = 0 },
            new { Id = "cylinder", Name = "Connect 4 Cylinder (7×6, Win 4)", Width = 7, Height = 6, Win = 4, Type = 1 }
        };
        
        ConfigurationSelectList = new SelectList(configurations, "Id", "Name");
    }
    
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadConfigurations();
            return Page();
        }
        
        int boardWidth, boardHeight, winCondition, boardType;
        
        switch (ConfigId.ToLower())
        {
            case "classic":
                boardWidth = 7;
                boardHeight = 6;
                winCondition = 4;
                boardType = 0;
                break;
            case "connect3":
                boardWidth = 5;
                boardHeight = 4;
                winCondition = 3;
                boardType = 0;
                break;
            case "connect5":
                boardWidth = 9;
                boardHeight = 7;
                winCondition = 5;
                boardType = 0;
                break;
            case "cylinder":
                boardWidth = 7;
                boardHeight = 6;
                winCondition = 4;
                boardType = 1;
                break;
            default:
                boardWidth = 7;
                boardHeight = 6;
                winCondition = 4;
                boardType = 0;
                break;
        }
        
        // Redirect to gameplay
        return RedirectToPage("./GamePlay", new
        {
            boardWidth = boardWidth,
            boardHeight = boardHeight,
            winCondition = winCondition,
            boardType = boardType,
            player1Name = Player1Name,
            player2Name = Player2Name,
            p1Type = P1Type,
            p2Type = P2Type
        });
    }
}