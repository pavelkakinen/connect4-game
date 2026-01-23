using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin.DanceStyles;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<DanceStyleViewModel> DanceStyles { get; set; } = new();

    public class DanceStyleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ClassCount { get; set; }
        public bool CanDelete => ClassCount == 0;
    }

    public async Task OnGetAsync()
    {
        var styles = await _context.DanceStyles
            .Include(s => s.DanceClasses)
            .OrderBy(s => s.Name)
            .ToListAsync();

        DanceStyles = styles.Select(s => new DanceStyleViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            ClassCount = s.DanceClasses.Count
        }).ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var style = await _context.DanceStyles
            .Include(s => s.DanceClasses)
            .Include(s => s.Packages)
            .Include(s => s.TrialUsages)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (style == null)
        {
            TempData["Error"] = "Dance style not found.";
            return RedirectToPage();
        }

        // Check if style is used in classes - this is the main constraint
        if (style.DanceClasses.Any())
        {
            TempData["Error"] = $"Cannot delete '{style.Name}' - it is used by {style.DanceClasses.Count} class(es).";
            return RedirectToPage();
        }

        // Remove related packages (MonthlyUnlimitedSingle packages for this style)
        if (style.Packages.Any())
        {
            _context.Packages.RemoveRange(style.Packages);
        }

        // Remove related trial usages
        if (style.TrialUsages.Any())
        {
            _context.TrialUsages.RemoveRange(style.TrialUsages);
        }

        var styleName = style.Name;
        _context.DanceStyles.Remove(style);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"'{styleName}' has been deleted.";
        return RedirectToPage();
    }
}
