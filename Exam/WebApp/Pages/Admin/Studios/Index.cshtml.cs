using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin.Studios;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<StudioViewModel> Studios { get; set; } = new();

    public class StudioViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SizeSquareMeters { get; set; }
        public int MaxCapacity { get; set; }
        public bool HasPoles { get; set; }
        public bool HasAerialRigging { get; set; }
        public int ClassCount { get; set; }
        public bool CanDelete => ClassCount == 0;
    }

    public async Task OnGetAsync()
    {
        var studios = await _context.Studios
            .Include(s => s.DanceClasses)
            .OrderBy(s => s.Name)
            .ToListAsync();

        Studios = studios.Select(s => new StudioViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            SizeSquareMeters = s.SizeSquareMeters,
            MaxCapacity = s.MaxCapacity,
            HasPoles = s.HasPoles,
            HasAerialRigging = s.HasAerialRigging,
            ClassCount = s.DanceClasses.Count
        }).ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var studio = await _context.Studios
            .Include(s => s.DanceClasses)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (studio == null)
        {
            TempData["Error"] = "Studio not found.";
            return RedirectToPage();
        }

        if (studio.DanceClasses.Any())
        {
            TempData["Error"] = $"Cannot delete '{studio.Name}' - it has {studio.DanceClasses.Count} scheduled class(es).";
            return RedirectToPage();
        }

        var studioName = studio.Name;
        _context.Studios.Remove(studio);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"'{studioName}' has been deleted.";
        return RedirectToPage();
    }
}
