using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class NewGame : PageModel
{
    private static readonly Dictionary<string, (int Width, int Height, int Win, int Type)> Configurations = new()
    {
        ["classic"] = (7, 6, 4, 0),
        ["connect3"] = (5, 4, 3, 0),
        ["connect5"] = (9, 7, 5, 0),
        ["cylinder"] = (7, 6, 4, 1)
    };

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
        var configList = new[]
        {
            new { Id = "classic", Name = "Classic Connect 4 (7×6, Win 4)" },
            new { Id = "connect3", Name = "Connect 3 (5×4, Win 3)" },
            new { Id = "connect5", Name = "Connect 5 (9×7, Win 5)" },
            new { Id = "cylinder", Name = "Connect 4 Cylinder (7×6, Win 4)" }
        };

        ConfigurationSelectList = new SelectList(configList, "Id", "Name");
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadConfigurations();
            return Page();
        }

        var key = ConfigId.ToLower();
        var cfg = Configurations.ContainsKey(key) ? Configurations[key] : Configurations["classic"];

        return RedirectToPage("./GamePlay", new
        {
            boardWidth = cfg.Width,
            boardHeight = cfg.Height,
            winCondition = cfg.Win,
            boardType = cfg.Type,
            player1Name = Player1Name,
            player2Name = Player2Name,
            p1Type = P1Type,
            p2Type = P2Type
        });
    }
}
