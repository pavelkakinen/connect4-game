using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Admin.Classes;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<DanceClass> Classes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? StyleFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? StudioFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DayOfWeek? DayFilter { get; set; }

    public SelectList StyleOptions { get; set; } = default!;
    public SelectList StudioOptions { get; set; } = default!;
    public SelectList DayOptions { get; set; } = default!;

    public async Task OnGetAsync()
    {
        // Load filter options
        var styles = await _context.DanceStyles.OrderBy(s => s.Name).ToListAsync();
        StyleOptions = new SelectList(styles, "Id", "Name");

        var studios = await _context.Studios.OrderBy(s => s.Name).ToListAsync();
        StudioOptions = new SelectList(studios, "Id", "Name");

        var days = new[]
        {
            new { Value = (int)DayOfWeek.Monday, Text = "Monday" },
            new { Value = (int)DayOfWeek.Tuesday, Text = "Tuesday" },
            new { Value = (int)DayOfWeek.Wednesday, Text = "Wednesday" },
            new { Value = (int)DayOfWeek.Thursday, Text = "Thursday" },
            new { Value = (int)DayOfWeek.Friday, Text = "Friday" },
            new { Value = (int)DayOfWeek.Saturday, Text = "Saturday" },
            new { Value = (int)DayOfWeek.Sunday, Text = "Sunday" }
        };
        DayOptions = new SelectList(days, "Value", "Text");

        // Build query
        var query = _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .AsQueryable();

        if (StyleFilter.HasValue)
            query = query.Where(c => c.DanceStyleId == StyleFilter.Value);

        if (StudioFilter.HasValue)
            query = query.Where(c => c.StudioId == StudioFilter.Value);

        if (DayFilter.HasValue)
            query = query.Where(c => c.DayOfWeek == DayFilter.Value);

        // SQLite doesn't support TimeSpan ordering, so we load then sort in memory
        var loadedClasses = await query
            .OrderBy(c => c.DayOfWeek)
            .ToListAsync();

        Classes = loadedClasses
            .OrderBy(c => c.DayOfWeek)
            .ThenBy(c => c.StartTime)
            .ThenBy(c => c.DanceStyle.Name)
            .ToList();
    }

    public string GetLevelBadgeClass(ClassLevel level)
    {
        return level switch
        {
            ClassLevel.Beginner => "bg-success",
            ClassLevel.Intermediate => "bg-primary",
            ClassLevel.Advanced => "bg-warning text-dark",
            ClassLevel.Professional => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
